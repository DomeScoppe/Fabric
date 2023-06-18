using Loom.GameDev.View;
using System.Windows;
using System.Windows.Controls;

namespace Loom.Editors
{
    public partial class LevelEditorView : UserControl
    {
        public LevelEditorView()
        {
            InitializeComponent();
            Loaded += OnLevelEditorLoaded;
        }

        private void OnLevelEditorLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLevelEditorLoaded;
            Focus();
        }

        private void OnNewScript_Click(object sender, RoutedEventArgs e)
        {
            new NewScriptDialog().ShowDialog();
        }
    }
}
