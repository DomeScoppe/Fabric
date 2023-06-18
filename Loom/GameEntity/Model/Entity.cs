using Loom.Core;
using Loom.DLLWrappers;
using Loom.GameProject.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Loom.GameEntity.Model
{
    [DataContract]
    [KnownType(typeof(Transform))]
    [KnownType(typeof(Script))]
    public class Entity : ViewModelBase
    {
        private bool _isEnabled = true;
        private string _name;
        private ulong _entityID = ID.INVALID_ID;
        private bool _isActive;

        public ulong EntityID
        {
            get => _entityID;
            set
            {
                if(_entityID != value)
                {
                    _entityID = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if(_isActive != value)
                {
                    _isActive = value;
                    if(_isActive)
                    {
                        EntityID = EngineAPI.EntityAPI.CreateEntity(this);
                        Debug.Assert(ID.IsValid(EntityID));
                    }
                    else if(ID.IsValid(EntityID))
                    {
                        EngineAPI.EntityAPI.RemoveEntity(this);
                        EntityID = ID.INVALID_ID;
                    }

                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Order = 1)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if(_isEnabled != value)
                {
                    _isEnabled = value;
                    
                    if(Project.Current != null) Project.Current.IsDirty = true;

                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Order = 2)]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;

                    if (Project.Current != null) Project.Current.IsDirty = true;

                    OnPropertyChanged();
                }
            }
        }

        public Scene ParentScene { get; set; }

        [DataMember(Order = 3, Name = "Components")]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public Component GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);

        public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

        public bool AddComponent(Component component)
        {
            Debug.Assert(component != null);
            if(!Components.Any(x=> x.GetType() == component.GetType()))
            {
                IsActive = false;
                _components.Add(component);
                IsActive = true;

                Project.Current.IsDirty = true;

                return true;
            }
            Logger.Log(MessageType.Warn, $"Entity {Name} already has a {component.GetType().Name} component");
            return false;
        }

        public void RemoveComponent(Component component)
        {
            Debug.Assert(component != null);
            if (component is Transform)
            {
                Logger.Log(MessageType.Warn, "Transform components can't be removed.");
                return;
            }
            if(_components.Contains(component))
            {
                IsActive = false;
                _components.Remove(component);
                IsActive = true;

                Project.Current.IsDirty = true;
            }

        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged();
            }
        }

        public Entity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;

            _components.Add(new Transform(this));

            OnDeserialized(new StreamingContext());
        }
    }

    public abstract class MSEntityBase : ViewModelBase
    {
        private bool _enableUpdates = true;
        private bool? _isEnabled;
        private string _name;

        public bool? IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    Project.Current.IsDirty = true;
                    OnPropertyChanged();
                }
            }
        }

        private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; }

        public T GetMSComponent<T>() where T: IMSComponent
        {
            return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        public List<Entity> SelectedEntities { get; }

        private void MakeComponentList()
        {
            _components.Clear();
            var firstEntity = SelectedEntities.FirstOrDefault();
            if (firstEntity == null) return;

            foreach(var component in firstEntity.Components)
            {
                var type = component.GetType();
                if(!SelectedEntities.Skip(1).Any(entity=>entity.GetComponent(type) == null))
                {
                    Debug.Assert(Components.FirstOrDefault(x => x.GetType() == type) == null);
                    _components.Add(component.GetMultiSelectionComponent(this));
                }
            }
        }

        public static float? GetMixedValue<T>(List<T> objects, Func<T, float> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x=> !getProperty(x).IsTheSameAs(value)) ? (float?) null : value;
        }

        public static bool? GetMixedValue<T>(List<T> objects, Func<T, bool> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => value != getProperty(x)) ? (bool?)null : value;
        }

        public static string GetMixedValue<T>(List<T> objects, Func<T, string> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => value != getProperty(x)) ? null : value;
        }

        protected virtual bool UpdateMSEntity()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<Entity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<Entity, string>(x => x.Name));

            return true;
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSEntity();
            MakeComponentList();
            _enableUpdates = true;
        }

        protected virtual bool UpdateEntities(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }

            return false;
        }

        public MSEntityBase(List<Entity> entities)
        {
            Debug.Assert(entities?.Any() == true);

            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedEntities = entities;

            PropertyChanged += (s, e) =>
            {
                if (_enableUpdates)
                {
                    UpdateEntities(e.PropertyName);
                }
            };
        }
    }

    public class MSEntity : MSEntityBase
    {
        public MSEntity(List<Entity> entities)
            : base(entities)
        {
            Refresh();
        }
    }
}
