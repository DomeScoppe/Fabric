using Loom.Core;
using Loom.GameEntity.Model;
using Loom.GameProject.Model;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace Loom.Editors
{
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChanged = false;

        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, Action<(Transform transform, Vector3)> forEachAction)
        {
            if (!(DataContext is MSTransform dc))
            {
                _undoAction = null;
                _propertyChanged = false;
                return null;
            }

            var selection = dc.SelectedComponents.Select(x=>selector(x)).ToList();
            return new Action(() =>
            {
                selection.ForEach(x=>forEachAction(x));
                (EntityInspectorView.Instance.DataContext as MSEntityBase)?.GetMSComponent<MSTransform>().Refresh();
            });
        }

        private Action GetPositionAction() => GetAction((x)=> (x, x.Position), (x)=> x.transform.Position = x.Item2);
        private Action GetRotationAction() => GetAction((x)=> (x, x.Rotation), (x)=> x.transform.Rotation = x.Item2);
        private Action GetScaleAction() => GetAction((x)=> (x, x.Scale), (x)=> x.transform.Scale = x.Item2);

        private void RecordAction(Action redoAction, string name)
        {
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;

                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }

        private void OnPosition_MouseLBD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _propertyChanged = false;

            _undoAction = GetPositionAction();
        }

        private void OnPosition_MouseLBU(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RecordAction(GetPositionAction(), "Position changed");
        }

        private void OnPosition_LostKbdFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            RecordAction(GetPositionAction(), "Position changed");
        }

        private void OnRotation_MouseLBD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _propertyChanged = false;

            _undoAction = GetRotationAction();
        }

        private void OnRotation_MouseLBU(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RecordAction(GetRotationAction(), "Rotation changed");
        }

        private void OnRotation_LostKbdFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            RecordAction(GetRotationAction(), "Rotation changed");
        }

        private void OnScale_MouseLBD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _propertyChanged = false;

            _undoAction = GetScaleAction();
        }

        private void OnScale_MouseLBU(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RecordAction(GetScaleAction(), "Scale changed");
        }

        private void OnScale_LostKbdFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            RecordAction(GetScaleAction(), "Scale changed");
        }
    }
}
