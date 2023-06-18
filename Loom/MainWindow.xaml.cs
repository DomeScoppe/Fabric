using Loom.GameProject;
using Loom.GameProject.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Loom
{
    public partial class MainWindow : Window
    {
        public static string FabricPath { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;

            this.WindowState = WindowState.Maximized;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;

            this.Hide();

            GetEnginePath();

            OpenProjectBrowser();
        }

        private void GetEnginePath()
        {
            var enginePath = Environment.GetEnvironmentVariable("FABRIC_ENGINE", EnvironmentVariableTarget.User);
            if (enginePath == null || !Directory.Exists(Path.Combine(enginePath, @"Fabric\EngineAPI")))
            {
                var dlg = new EnginePathDialog();
                if(dlg.ShowDialog() == true)
                {
                    FabricPath = dlg.FabricPath;
                    Environment.SetEnvironmentVariable("FABRIC_ENGINE", FabricPath.ToUpper(), EnvironmentVariableTarget.User);
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                FabricPath = enginePath;
            }
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            if(Application.Current.MainWindow == this && Project.Current != null && Project.Current.IsDirty)
            {
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Information;
                MessageBoxResult result;

                result = MessageBox.Show("Do you want to save changes to project?", Project.Current?.ProjectName, button, icon, MessageBoxResult.Cancel);

                if (result == MessageBoxResult.Yes)
                {
                    Project.Save(Project.Current);
                }

                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            Closing -= OnMainWindowClosing;

            Project.Current?.Unload();
        }

        private void OpenProjectBrowser()
        {
            var projectBrowser = new ProjectBrowserView();
            
            Application.Current.MainWindow = projectBrowser;

            if(projectBrowser.ShowDialog() == true || projectBrowser.DataContext != null)
            {
                if(Application.Current != null)
                {
                    Application.Current.MainWindow = this;

                    Project.Current?.Unload();
                    DataContext = projectBrowser.DataContext;

                    this.Show();
                }
            }
        }
        private void OnSaveMenu_Click(object sender, RoutedEventArgs e)
        {
            if(Project.Current != null)
            {
                Project.Save(Project.Current);
            }
        }
    }
}
