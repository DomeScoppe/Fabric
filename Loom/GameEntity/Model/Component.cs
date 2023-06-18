using Loom.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Loom.GameEntity.Model
{
    public interface IMSComponent
    {

    }

    [DataContract]
    abstract public class Component : ViewModelBase
    {
        public Entity Owner { get; private set; }

        public abstract IMSComponent GetMultiSelectionComponent(MSEntityBase msEntity);
        public abstract void WriteToBinary(BinaryWriter bw);

        public Component(Entity entity) 
        {
            Debug.Assert(entity != null);
            Owner = entity;
        }
    }

    abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
    {
        private bool _enableUpdates = true;

        public List<T> SelectedComponents { get; }

        protected abstract bool UpdateComponents(string propertyName);

        protected abstract bool UpdateMSComponent();

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSComponent();
            _enableUpdates = true;
        }

        public MSComponent(MSEntityBase msEntity)
        {
            Debug.Assert(msEntity?.SelectedEntities?.Any() == true);

            SelectedComponents = msEntity.SelectedEntities.Select(entity => entity.GetComponent<T>()).ToList();

            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateComponents(e.PropertyName); };
        }
    }
}
