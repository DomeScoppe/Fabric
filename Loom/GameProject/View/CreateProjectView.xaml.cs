using Loom.Core;
using Loom.GameProject.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Loom.GameProject.View
{
    public partial class CreateProjectView : UserControl
    {
        public CreateProjectView()
        {
            InitializeComponent();
        }

        private void OnCreateButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var dc = DataContext as CreateProjectViewModel;
            var projectPath = dc.CreateProject(TemplateListBox.SelectedItem as ProjectTemplate);

            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if(!string.IsNullOrEmpty(projectPath))
            {
                dialogResult = true;
                var project = OpenProjectViewModel.Open(new ProjectData() { ProjectName = dc.ProjectName, ProjectPath = dc.ProjectPath });
                win.DataContext = project;
            }

            win.DialogResult = dialogResult;
            win.Close();
        }

        private void OnBrowseButtonClicked(object sender, RoutedEventArgs e)
        {
            FolderBrowser.Browse(PathTextField);
        }
    }
}
