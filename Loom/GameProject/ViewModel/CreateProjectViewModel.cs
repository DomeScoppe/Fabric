using Loom.Core;
using Loom.GameProject.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Loom.GameProject.ViewModel
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }

        [DataMember]
        public string ProjectFile { get; set; }

        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }

        public string IconPath { get; set; }

        public byte[] Screenshot { get; set; }

        public string ScreenshotPath { get; set; }

        public string ProjectFilePath { get; set; }

        public string TemplatePath { get; set; }
    }

    class CreateProjectViewModel : ViewModelBase
    {
        //TODO: Get template path from installation directory
        private readonly string _templatePath = @"..\..\Loom\ProjectTemplates\";
        private string _name = "New Project";

        public string ProjectName 
        { 
            get => _name;
            set 
            { 
                if(_name != value)
                {
                    _name = value;
                    ValidateProjectPath();
                    OnPropertyChanged();
                }
            }
        }

        private string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Fabric Projects\";

        public string ProjectPath
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    ValidateProjectPath();
                    OnPropertyChanged();
                }
            }
        }

        private bool _isValid = false;

        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            path += $@"{ProjectName}\";

            IsValid = false;
            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMessage = "Type in a project name.";
            }
            else if(ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMessage = "Invalid character(s) used in project name.";
            }
            else if(string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMessage = "Invalid location for project.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMessage = "Invalid character(s) used in project path.";
            }
            else if(Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMessage = "Selected project folder already exists.";
            }
            else
            {
                ErrorMessage = string.Empty;
                IsValid = true;
            }

            return IsValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectPath();

            if(!IsValid)
            {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
            var path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                if(!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach(var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }

                var dirInfo = new DirectoryInfo(path + @".fabric\");
                dirInfo.Attributes |= FileAttributes.Hidden;

                File.Copy(template.IconPath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "icon.png")));
                File.Copy(template.ScreenshotPath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "screenshot.png")));

                var project = new Project(ProjectName, path);

                Serializer.Serialize(project, path + $"{ProjectName}{Project.Extension}");

                CreateSolution(template, path);

                return path;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create {ProjectName}");

                throw;
            }
        }

        private void CreateSolution(ProjectTemplate template, string projectPath)
        {
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "GameSolution")));
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "GameProject")));

            var engineAPIPath = Path.Combine(MainWindow.FabricPath, @"Fabric\EngineAPI\");
            Debug.Assert(Directory.Exists(engineAPIPath));

            var _0 = ProjectName;
            var _1 = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            var _2 = engineAPIPath;
            var _3 = MainWindow.FabricPath;

            var solution = File.ReadAllText(Path.Combine(template.TemplatePath, "GameSolution"));
            solution = string.Format(solution, _0, _1, "{" + Guid.NewGuid().ToString().ToUpper() + "}");
            File.WriteAllText(Path.GetFullPath(Path.Combine(projectPath, $"{_0}.sln")), solution);

            var project = File.ReadAllText(Path.Combine(template.TemplatePath, "GameProject"));
            project = string.Format(project, _0, _1, _2, _3);
            File.WriteAllText(Path.GetFullPath(Path.Combine(projectPath, $@"Source\{_0}.vcxproj")), project);
        }

        public CreateProjectViewModel()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);

            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.json", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var templateFile in templateFiles) 
                {
                    var template = Serializer.Deserialize<ProjectTemplate>(templateFile);

                    template.IconPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), "icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconPath);

                    template.ScreenshotPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), "screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotPath);

                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), template.ProjectFile));
                    template.TemplatePath = Path.GetDirectoryName(templateFile);

                    _projectTemplates.Add(template);
                }

                ValidateProjectPath();
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex.Message);

                Logger.Log(MessageType.Error, ex.Message);

                throw;
            }

        }
    }
}
