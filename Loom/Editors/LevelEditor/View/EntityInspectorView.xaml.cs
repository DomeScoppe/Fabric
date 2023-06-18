using Loom.Core;
using Loom.GameEntity.Model;
using Loom.GameProject.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Loom.Editors
{
    public class NullableBoolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b == true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b == true;
        }
    }

    public partial class EntityInspectorView : UserControl
    {
        private string _propertyName;
        private Action _undoAction;

        public static EntityInspectorView Instance { get; private set; }

        public EntityInspectorView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;

            DataContextChanged += (_, __) =>
            {
                if (DataContext != null)
                {
                    (DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                }
            };
        }

        private Action GetRenameAction()
        {
            var dc = DataContext as MSEntity;
            var selection = dc.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();

            return new Action(() =>
            {
                selection.ForEach(item => item.entity.Name = item.Name);
                (DataContext as MSEntity).Refresh();
            });
        }

        private void OnName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _propertyName = string.Empty;
            _undoAction = GetRenameAction();
        }

        private void OnName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(_propertyName == nameof(MSEntity.Name) && _undoAction != null)
            {
                var redoAction = GetRenameAction();

                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity"));
                _propertyName = null;
            }
            _undoAction = null;
        }

        private Action GetIsEnabledAction()
        {
            var dc = DataContext as MSEntity;
            var selection = dc.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();

            return new Action(() =>
            {
                selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                (DataContext as MSEntity).Refresh();
            });
        }

        private void OnIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            var undoAction = GetIsEnabledAction();
            var dc = DataContext as MSEntity;
            dc.IsEnabled = (sender as CheckBox).IsChecked == true;

            var redoAction = GetIsEnabledAction();

            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction, dc.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
        }

        private void OnAddComponent_Button_Preview_MouseLBD(object sender, MouseButtonEventArgs e)
        {
            var menu = FindResource("addComponentMenu") as ContextMenu;
            var btn = sender as ToggleButton;
            btn.IsChecked = true;
            menu.Placement = PlacementMode.Bottom;
            menu.PlacementTarget = btn;
            menu.MinWidth = btn.ActualWidth;
            menu.IsOpen = true;
        }

        private void OnAddScriptComponent(object sender, RoutedEventArgs e)
        {
            AddComponent(ComponentType.Script, (sender as MenuItem).Header.ToString());
        }

        private void AddComponent(ComponentType componentType, object data)
        {
            var creationFunction = ComponentFactory.GetCreationFunction(componentType);
            var changedEntities = new List<(Entity entity, Component component)>();
            var dc = DataContext as MSEntity;
            foreach(var entity in dc.SelectedEntities)
            {
                var component = creationFunction(entity, data);
                if(entity.AddComponent(component))
                {
                    changedEntities.Add((entity, component));
                }
            }

            if(changedEntities.Any())
            {
                dc.Refresh();

                Project.UndoRedo.Add(new UndoRedoAction(
                    ()=>
                    {
                        changedEntities.ForEach(x => x.entity.RemoveComponent(x.component));
                        (DataContext as MSEntity).Refresh();
                    },
                    () =>
                    {
                        changedEntities.ForEach(x => x.entity.AddComponent(x.component));
                        (DataContext as MSEntity).Refresh();
                    },
                    $"Add {componentType} component"));
            }
        }
    }
}
