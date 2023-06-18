using Loom.Core;
using Loom.GameEntity.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Loom.GameProject.Model
{
    [DataContract]
    public class Scene : ViewModelBase
    {
        private static Project _project;
        private string _name;
        private bool _isActive;

        public static Project Project => _project;

        public string FullPath { get; set; }

        [DataMember(Order = 1)]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Order = 2)]
        public string SceneFile { get; set; }

        [DataMember(Order = 3)]
        public bool IsActive
        {
            get => _isActive;
            set 
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Order = 4, Name = "Entities")]
        private readonly ObservableCollection<Entity> _entities = new ObservableCollection<Entity>();
        public ReadOnlyObservableCollection<Entity> Entities { get; private set; }

        public ICommand AddEntityCommand { get; private set; }

        public ICommand RemoveEntityCommand { get; private set; }

        public static void SaveScene(Scene scene)
        {
            Serializer.Serialize(scene, scene.FullPath);
        }

        public static Scene LoadScene(Project project, string sceneFile)
        {
            Debug.Assert(project != null);
            _project = project;

            Scene scene = Serializer.Deserialize<Scene>(sceneFile);
            scene.FullPath = sceneFile;

            return scene;
        }

        private void AddEntity(Entity entity, int index = -1)
        {
            Debug.Assert(!_entities.Contains(entity));
            entity.IsActive = IsActive;
            if(index == -1)
            {
                _entities.Add(entity);
            }
            else
            {
                _entities.Insert(index, entity);
            }

            Project.IsDirty = true;
        }

        private void RemoveEntity(Entity entity)
        {
            Debug.Assert(_entities.Contains(entity));
            entity.IsActive = false;
            _entities.Remove(entity);

            Project.IsDirty = true;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Entities = new ReadOnlyObservableCollection<Entity>(_entities);

            foreach (var entity in Entities)
            {
                entity.IsActive = IsActive;
                entity.ParentScene = this;
            }

            AddEntityCommand = new RelayCommand<Entity>(x =>
            {
                AddEntity(x);
                var entityIndex = _entities.Count - 1;

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveEntity(x),
                    () => AddEntity(x, entityIndex),
                    $"Add {x.Name} to {Name}"));
            });

            RemoveEntityCommand = new RelayCommand<Entity>(x =>
            {
                var entityIndex = _entities.IndexOf(x);
                RemoveEntity(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => AddEntity(x, entityIndex),
                    () => RemoveEntity(x),
                    $"Remove {x.Name} from {Name}"));
            });
        }

        public Scene(Project project, string name, bool isActive = false)
        {
            Debug.Assert(project != null);
            Debug.Assert(!string.IsNullOrEmpty(name));

            Entities = new ReadOnlyObservableCollection<Entity>(_entities);

            _project = project;

            Name = name;
            _isActive = isActive;

            SceneFile = $@"{Name}.fscene";

            FullPath = $@"{project.ProjectPath}\Assets\Scenes\{SceneFile}";

            Serializer.Serialize(this, FullPath);

            OnDeserialized(new StreamingContext());
        }
    }
}
