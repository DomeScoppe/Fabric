using System;
using System.Diagnostics;

namespace Loom.GameEntity.Model
{
    enum ComponentType
    {
        Transform,
        Script,
    }

    static class ComponentFactory
    {
        private static readonly Func<Entity, object, Component>[] _function
            = new Func<Entity, object, Component>[]
            {
                (entity, data)=>new Transform(entity),
                (entity, data)=>new Script(entity){ Name = (string)data },
            };

        public static Func<Entity, object, Component> GetCreationFunction(ComponentType componentType)
        {
            Debug.Assert((int)componentType < _function.Length);
            return _function[(int)componentType];
        }

        public static ComponentType ToEnumType(this Component component)
        {
            return component switch
            {
                Transform _ => ComponentType.Transform,
                Script _ => ComponentType.Script,
                _ => throw new ArgumentException("Unknown component type"),
            };
        }
    }
}
