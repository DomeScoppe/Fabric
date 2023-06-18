using Loom.GameProject.Model;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Loom.GameEntity.Model
{
    [DataContract]
    class Script : Component
    {
        private string _name;

        [DataMember]
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

        public override IMSComponent GetMultiSelectionComponent(MSEntityBase msEntity) => new MSScript(msEntity);

        public override void WriteToBinary(BinaryWriter bw)
        {
            var nameBytes = Encoding.UTF8.GetBytes(Name);
            bw.Write(nameBytes.Length);
            bw.Write(nameBytes);
        }

        public Script(Entity entity) : base(entity)
        {

        }
    }

    sealed class MSScript : MSComponent<Script>
    {
        private string _name;

        [DataMember]
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

        protected override bool UpdateComponents(string propertyName)
        {
            if(propertyName == nameof(Name))
            {
                SelectedComponents.ForEach(c => c.Name = _name);
                return true;
            }

            return false;
        }

        protected override bool UpdateMSComponent()
        {
            Name = MSEntity.GetMixedValue(SelectedComponents, new Func<Script, string>(x => x.Name));
            return true;
        }

        public MSScript(MSEntityBase msEntity) : base(msEntity)
        {
            Refresh();
        }
    }
}
