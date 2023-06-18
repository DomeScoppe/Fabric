using Loom.Core;
using Loom.GameProject.Model;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;

namespace Loom.GameEntity.Model
{
    [DataContract]
    class Transform : Component
    {
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale = new Vector3(1, 1, 1);

        [DataMember(Order = 1)]
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;

                    if (Project.Current != null) Project.Current.IsDirty = true;

                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Order = 2)]
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;

                    if (Project.Current != null) Project.Current.IsDirty = true;

                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Order = 3)]
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;

                    if (Project.Current != null) Project.Current.IsDirty = true;

                    OnPropertyChanged();
                }
            }
        }

        public override IMSComponent GetMultiSelectionComponent(MSEntityBase msEntity) => new MSTransform(msEntity);

        public override void WriteToBinary(BinaryWriter bw)
        {
            bw.Write(_position.X); bw.Write(_position.Y); bw.Write(_position.Z);
            bw.Write(_rotation.X); bw.Write(_rotation.Y); bw.Write(_rotation.Z);
            bw.Write(_scale.X); bw.Write(_scale.Y); bw.Write(_scale.Z);
        }

        public Transform(Entity entity) : base(entity)
        {
        }
    }

    sealed class MSTransform : MSComponent<Transform>
    {
        private float? _positionX;
        private float? _positionY;
        private float? _positionZ;
        private float? _rotationX;
        private float? _rotationY;
        private float? _rotationZ;
        private float? _scaleX;
        private float? _scaleY;
        private float? _scaleZ;

        public float? PositionX
        {
            get => _positionX;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _positionX))
                    _positionX = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        public float? PositionY
        {
            get => _positionY;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _positionY))
                    _positionY = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        public float? PositionZ
        {
            get => _positionZ;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _positionZ))
                    _positionZ = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        public float? RotationX
        {
            get => _rotationX;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _rotationX))
                    _rotationX = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        public float? RotationY
        {
            get => _rotationY;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _rotationY))
                    _rotationY = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        public float? RotationZ
        {
            get => _rotationZ;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _rotationZ))
                    _rotationZ = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        public float? ScaleX
        {
            get => _scaleX;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _scaleX))
                    _scaleX = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        public float? ScaleY
        {
            get => _scaleY;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _scaleY))
                    _scaleY = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        public float? ScaleZ
        {
            get => _scaleZ;
            set
            {
                if (!MathUtilities.IsTheSameAs(value, _scaleZ))
                    _scaleZ = value;
                Project.Current.IsDirty = true;
                OnPropertyChanged();
            }
        }

        protected override bool UpdateComponents(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(PositionX):
                case nameof(PositionY):
                case nameof(PositionZ):
                    SelectedComponents.ForEach(c => c.Position = new Vector3(_positionX ?? c.Position.X, _positionY ?? c.Position.Y, _positionZ ?? c.Position.Z));
                    return true;

                case nameof(RotationX):
                case nameof(RotationY):
                case nameof(RotationZ):
                    SelectedComponents.ForEach(c => c.Rotation = new Vector3(_rotationX ?? c.Rotation.X, _rotationY ?? c.Rotation.Y, _rotationZ ?? c.Rotation.Z));
                    return true;

                case nameof(ScaleX):
                case nameof(ScaleY):
                case nameof(ScaleZ):
                    SelectedComponents.ForEach(c => c.Scale = new Vector3(_scaleX ?? c.Scale.X, _scaleY ?? c.Scale.Y, _scaleZ ?? c.Scale.Z));
                    return true;
            }

            return false;
        }

        protected override bool UpdateMSComponent()
        {
            PositionX = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.X));
            PositionY = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.Y));
            PositionZ = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.Z));

            RotationX = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.X));
            RotationY = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.Y));
            RotationZ = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.Z));

            ScaleX = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.X));
            ScaleY = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.Y));
            ScaleZ = MSEntityBase.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.Z));

            return true;
        }

        public MSTransform(MSEntityBase msEntity) : base(msEntity)
        {
            Refresh();
        }
    }
}
