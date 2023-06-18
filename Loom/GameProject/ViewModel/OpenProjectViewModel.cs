using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using Loom.GameProject.Model;
using System.Linq;
using Loom.Core;

namespace Loom.GameProject.ViewModel
{
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }

        [DataMember]
        public string ProjectPath { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        public string FullPath { get => $"{ProjectPath}{ProjectName}\\{ProjectName}{Project.Extension}"; }

        public byte[] Icon { get; set; }

        public byte[] Screenshot { get; set; }
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }

    class OpenProjectViewModel
    {
        private static readonly string _appDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Loom\";

        private static readonly string _projectDataPath;

        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        public bool HasProjects { get => _projects.Any(); }

        private static void ReadProjectData()
        {
            if(File.Exists(_projectDataPath))
            {
                var projects = Serializer.Deserialize<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date);

                _projects.Clear();

                foreach(var project in projects)
                {
                    if(File.Exists(project.FullPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}{project.ProjectName}\.fabric\icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}{project.ProjectName}\.fabric\screenshot.png");

                        _projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            var projects = _projects.OrderBy(x => x.Date).ToList();

            Serializer.Serialize(new ProjectDataList() { Projects = projects }, _projectDataPath) ;
        }

        public static Project Open(ProjectData projectData)
        {
            ReadProjectData();

            var project = _projects.FirstOrDefault(x => x.FullPath == projectData.FullPath);

            if(project != null)
            {
                project.Date = DateTime.Now;
            }
            else
            {
                project = projectData;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }

            WriteProjectData();

            return Project.Load(project.FullPath);
        }

        static OpenProjectViewModel()
        {
            try
            {
                if(!Directory.Exists(_appDataPath)) Directory.CreateDirectory(_appDataPath);

                _projectDataPath = $@"{_appDataPath}ProjectData.json";

                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);

                ReadProjectData();
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, "Failed to read project data");

                throw;
            }
        }
    }
}
