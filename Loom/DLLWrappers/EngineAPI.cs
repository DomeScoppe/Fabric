using Loom.Core;
using Loom.EngineAPIStructs;
using Loom.GameEntity.Model;
using Loom.GameProject.Model;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Loom.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    [StructLayout(LayoutKind.Sequential)]
    class ScriptComponent
    {
        public IntPtr ScriptCreator;
    }

    [StructLayout(LayoutKind.Sequential)]
    class EntityDescription
    {
        public TransformComponent Transform = new TransformComponent();
        public ScriptComponent Script = new ScriptComponent();
    }
}

namespace Loom.DLLWrappers
{
    static class EngineAPI
    {
        private const string _engineDLL = "EngineDLL.dll";

        [DllImport(_engineDLL, CharSet = CharSet.Ansi)]
        public static extern int LoadGame(string path);

        [DllImport(_engineDLL)]
        public static extern int UnloadGame();

        [DllImport(_engineDLL)]
        public static extern IntPtr GetScriptCreator(string name);

        [DllImport(_engineDLL)]
        [return: MarshalAs(UnmanagedType.SafeArray)]
        public static extern string[] GetScriptNames();

        internal static class EntityAPI
        {
            [DllImport(_engineDLL)]
            private static extern ulong CreateEntity(EntityDescription desc);

            public static ulong CreateEntity(Entity entity)
            {
                EntityDescription desc = new EntityDescription();

                // Transform
                {
                    var c = entity.GetComponent<Transform>();
                    desc.Transform.Position = c.Position;
                    desc.Transform.Rotation = c.Rotation;
                    desc.Transform.Scale = c.Scale;
                }

                // Script component
                {
                    var c = entity.GetComponent<Script>();
                    if(c != null && Project.Current != null)
                    {
                        if(Project.Current.AvailableScripts.Contains(c.Name))
                        {
                            desc.Script.ScriptCreator = GetScriptCreator(c.Name);
                        }
                        else
                        {
                            Logger.Log(MessageType.Error, $"Unable to find script with name {c.Name}. Game entity will be created without script component!");
                        }
                    }
                }

                return CreateEntity(desc);
            }

            [DllImport(_engineDLL)]
            private static extern void RemoveEntity(ulong id);

            public static void RemoveEntity(Entity entity)
            {
                RemoveEntity(entity.EntityID);
            }
        }



    }
}
