using Loom.GameProject.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Loom.GameProject.View
{
    public partial class OpenProjectView : UserControl
    {
        public OpenProjectView()
        {
            InitializeComponent();
        }

        private void OnOpenButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var project = OpenProjectViewModel.Open(ProjectsListBox.SelectedItem as ProjectData);

            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (project != null)
            {
                dialogResult = true;
                win.DataContext = project;
            }

            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
