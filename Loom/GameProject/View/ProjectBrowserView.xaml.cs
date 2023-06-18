using Loom.GameProject.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace Loom.GameProject
{
    public partial class ProjectBrowserView : Window
    {
        public ProjectBrowserView()
        {
            InitializeComponent();

            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Closing -= OnWindowClosing;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if(sender == MinimizeButton)
            {
                this.WindowState = WindowState.Minimized;
            }
            if(sender == CloseButton)
            {
                Application.Current.Shutdown();
            }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as ProjectBrowserViewModel;

            if(dc != null)
            {
                if(sender == OpenProjectButton) dc.OpenProjectViewCommand.Execute(this);
                if(sender == CreateProjectButton) dc.CreateProjectViewCommand.Execute(this);
            }
        }
    }
}
