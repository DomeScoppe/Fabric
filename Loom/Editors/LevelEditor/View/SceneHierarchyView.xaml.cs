using Loom.Core;
using Loom.GameEntity.Model;
using Loom.GameProject.Model;
using System.Linq;
using System.Windows.Controls;

namespace Loom.Editors
{
    partial class SceneHierarchyView : UserControl
    {
        public Scene ActiveScene { get; set; }

        public SceneHierarchyView()
        {
            InitializeComponent();

            PopulateEntities();
        }

        private void PopulateEntities()
        {
            var project = DataContext as Project;

            if (project != null)
            {
                ActiveScene = project.Scenes.First(x => x.Name == project.ActiveScene);

                if (ActiveScene != null)
                {
                    foreach(Entity entity in ActiveScene.Entities)
                    {
                        TreeViewItem entityNode = new TreeViewItem();
                        entityNode.Header = entity.Name;
                    }
                }
            }

        }

        private void OnAddEntity_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            var project = menu.DataContext as Project;
            var dc = project.CurrentScene as Scene;

            dc.AddEntityCommand.Execute(new Entity(dc) { Name = "Empty entity" });
        }

        private void OnEntitySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var newSelection = listBox.SelectedItems.Cast<Entity>().ToList();
            var previousSelection = newSelection.Except(e.AddedItems.Cast<Entity>()).Concat(e.RemovedItems.Cast<Entity>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction(
                () => 
                {
                    listBox.UnselectAll();
                    previousSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                },
                () => 
                {
                    listBox.UnselectAll();
                    newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                },
                "Selection changed"));

            MSEntity msEntity = null;

            if(newSelection.Any())
            {
                msEntity = new MSEntity(newSelection);
            }

            EntityInspectorView.Instance.DataContext = msEntity;
        }
    }
}
