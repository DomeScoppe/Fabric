using Loom.Core;
using Loom.DLLWrappers;
using Loom.GameDev.Model;
using Loom.GameEntity.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Loom.GameProject.Model
{
    public enum BuildConfiguration
    {
        Debug,
        DebugEditor,
        Release,
        ReleaseEditor,
    }

    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        private static readonly string[] _buildConfigurationNames = new string[] { "Debug", "DebugEditor", "Release", "ReleaseEditor" };

        private int _buildConfig;

        private string[] _availableScripts;

        private bool _isDirty = false;

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged();
                }
            }
        }

        public string[] AvailableScripts
        {
            get => _availableScripts;
            set
            {
                if (_availableScripts != value)
                {
                    _availableScripts = value;
                    OnPropertyChanged();
                }
            }
        }

        public static string Extension { get; } = ".fabric";

        [DataMember(Order = 1)]
        public string ProjectName { get; private set; }

        [DataMember(Order = 2)]
        public string ProjectPath { get; private set; }

        private string _activeScene;

        [DataMember(Order = 3)]
        public string ActiveScene 
        { 
            get => _activeScene;
            private set 
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Order = 4)]
        public int BuildConfig
        {
            get => _buildConfig;
            set
            {
                if (_buildConfig != value)
                {
                    _buildConfig = value;
                    IsDirty = true;
                    OnPropertyChanged();
                }
            }
        }

        public Scene CurrentScene { get; private set; }

        public string FullPath => $"{ProjectPath}{ProjectName}{Extension}";

        public string Solution => $@"{ProjectPath}{ProjectName}.sln";

        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        [DataMember(Order = 5, Name = "Scenes")]
        public List<string> SceneNames { get; private set; } = new List<string>();

        public static Project Current => Application.Current?.MainWindow.DataContext as Project;

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        public ICommand AddSceneCommand { get; private set; }

        public ICommand RemoveSceneCommand { get; private set; }

        public ICommand UndoCommand { get; private set; }

        public ICommand RedoCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand BuildCommand { get; private set; }

        public ICommand DebugStartCommand { get; private set; }

        public ICommand DebugStartWithoutDebuggingCommand { get; private set; }

        public ICommand DebugStopCommand { get; private set; }

        public BuildConfiguration StandAloneBuildConfiguration => BuildConfig == 0 ? BuildConfiguration.Debug : BuildConfiguration.Release;
        public BuildConfiguration DLLBuildConfiguration => BuildConfig == 0 ? BuildConfiguration.DebugEditor : BuildConfiguration.ReleaseEditor;

        private static string GetConfigurationName(BuildConfiguration buildConfiguration) => _buildConfigurationNames[((int)buildConfiguration)];

        private void SetCommands()
        {
            AddSceneCommand = new RelayCommand<object>(x =>
            {
                AddScene($"New Scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;

                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => {
                        _scenes.Insert(sceneIndex, newScene);
                        SceneNames.Insert(sceneIndex, newScene.Name);
                    },
                    $"Add {newScene.Name}"));
            });

            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveScene(x);

                UndoRedo.Add(new UndoRedoAction(
                    () => {
                        _scenes.Insert(sceneIndex, x);
                        SceneNames.Insert(sceneIndex, x.Name);
                    },
                    () => RemoveScene(x),
                    $"Remove {x.Name}"));
            }, x => !x.IsActive);

            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo(), x => UndoRedo.UndoList.Any());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo(), x => UndoRedo.RedoList.Any());
            SaveCommand = new RelayCommand<object>(x => Save(this));
            BuildCommand = new RelayCommand<bool>(async x => await BuildGameDLL(x), x => !VisualStudio.IsDebugging() && VisualStudio.BuildDone);
            DebugStartCommand = new RelayCommand<object>(async x=> await RunGame(true), x=> !VisualStudio.IsDebugging() && VisualStudio.BuildDone);
            DebugStartWithoutDebuggingCommand = new RelayCommand<object>(async x=> await RunGame(false), x=> !VisualStudio.IsDebugging() && VisualStudio.BuildDone);
            DebugStopCommand = new RelayCommand<object>(async x=> await StopGame(), x=> VisualStudio.IsDebugging());

            OnPropertyChanged(nameof(AddSceneCommand));
            OnPropertyChanged(nameof(RemoveSceneCommand));
            OnPropertyChanged(nameof(UndoCommand));
            OnPropertyChanged(nameof(RedoCommand));
            OnPropertyChanged(nameof(SaveCommand));
            OnPropertyChanged(nameof(BuildCommand));
            OnPropertyChanged(nameof(DebugStartCommand));
            OnPropertyChanged(nameof(DebugStartWithoutDebuggingCommand));
            OnPropertyChanged(nameof(DebugStopCommand));
        }

        private void AddScene(string sceneName)
        {
            _scenes.Add(new Scene(this, sceneName));
            SceneNames.Add(sceneName);

            IsDirty = true;
        }

        private void RemoveScene(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
            SceneNames.Remove(scene.Name);

            IsDirty = true;
        }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));

            return Serializer.Deserialize<Project>(file);
        }

        public static void Save(Project project) 
        {
            foreach(var scene in project.Scenes)
            {
                Scene.SaveScene(scene);
            }

            Serializer.Serialize(project, project.FullPath);
            project.IsDirty = false;

            Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
        }

        private void SaveToBinary()
        {
            var configName = GetConfigurationName(StandAloneBuildConfiguration);
            var bin = $@"{ProjectPath}x64\{configName}\game.bin";

            using (var bw = new BinaryWriter(File.Open(bin, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(CurrentScene.Entities.Count);
                foreach(var entity in CurrentScene.Entities)
                {
                    bw.Write(0); // Entity type (reserved for later)
                    bw.Write(entity.Components.Count);
                    foreach(var component in entity.Components)
                    {
                        bw.Write((int)component.ToEnumType());
                        component.WriteToBinary(bw);
                    }
                }
            }
        }

        public void Unload()
        {
            UnloadGame();
            VisualStudio.CloseVisualStudio();
            UndoRedo.Reset();
        }

        private void LoadScenes()
        {
            var scenePath = Path.Combine(ProjectPath, "Assets\\Scenes\\");
            Debug.Assert(Directory.Exists(scenePath));

            foreach (var sceneName in SceneNames)
            {
                var scene = Path.GetFullPath(Path.Combine(scenePath, $"{sceneName}.fscene"));
                _scenes.Add(Scene.LoadScene(this, scene));
            }

            Scene activeScene = Scenes.FirstOrDefault(x => x.IsActive);
            ActiveScene = activeScene.Name;
            CurrentScene = activeScene;
        }

        [OnDeserialized]
        private async void OnDeserialized(StreamingContext context)
        {
            if(_scenes == null)
            {
                _scenes = new ObservableCollection<Scene>();
            }

            Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);

            OnPropertyChanged();

            LoadScenes();

            Debug.Assert(CurrentScene != null);

            await BuildGameDLL(false);

            SetCommands();
        }

        private async Task RunGame(bool debug)
        {
            var configName = GetConfigurationName(StandAloneBuildConfiguration);
            await Task.Run(() => VisualStudio.BuildSolution(this, configName, debug));
            if(VisualStudio.BuildSucceded)
            {
                SaveToBinary();
                await Task.Run(() => VisualStudio.Run(this, configName, debug));
            }
        }

        private async Task StopGame() => await Task.Run(() => VisualStudio.Stop());

        private async Task BuildGameDLL(bool showWindow = true)
        {
            try
            {
                UnloadGame();
                await Task.Run(() => VisualStudio.BuildSolution(this, GetConfigurationName(DLLBuildConfiguration), showWindow));
                if(VisualStudio.BuildSucceded)
                {
                    LoadGame();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private void LoadGame()
        {
            var configName = GetConfigurationName(DLLBuildConfiguration);
            var dll = $@"{ProjectPath}x64\{configName}\{ProjectName}.dll";

            AvailableScripts = null;

            if(File.Exists(dll) && EngineAPI.LoadGame(dll) != 0)
            {
                AvailableScripts = EngineAPI.GetScriptNames();
                CurrentScene.Entities.Where(x => x.GetComponent<Script>() != null).ToList().ForEach(x => x.IsActive = true);
                Logger.Log(MessageType.Info, "Game loaded successfully.");
            }
            else
            {
                Logger.Log(MessageType.Warn, "Failed to load game. Try to build the project first.");
            }
        }

        private void UnloadGame()
        {
            CurrentScene.Entities.Where(x => x.GetComponent<Script>() != null).ToList().ForEach(x => x.IsActive = false);
            if(EngineAPI.UnloadGame() != 0)
            {
                AvailableScripts = null;
                Logger.Log(MessageType.Info, "Game unloaded.");
            }
        }

        public Project(string name, string path) 
        {
            Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);

            ProjectName = name;
            ProjectPath = path;

            Scene scene = new Scene(this, "Default Scene", true);

            _scenes.Add(scene);
            SceneNames.Add(scene.Name);

            Scene activeScene = _scenes.First(x => x.IsActive);
            ActiveScene = activeScene.Name;
            CurrentScene = activeScene;

            OnDeserialized(new StreamingContext());
        }
    }
}
