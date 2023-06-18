using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Loom.Core
{
    public interface IUndoRedo
    {
        string Name { get; }

        void Undo();

        void Redo();
    }

    public class UndoRedoAction : IUndoRedo
    {
        private Action _undoAction;
        private Action _redoAction;

        public string Name { get; }

        public void Redo() => _redoAction();

        public void Undo() => _undoAction();

        public UndoRedoAction(string name)
        {
            Name = name;
        }

        public UndoRedoAction(Action undo, Action redo, string name)
            : this(name)
        {
            Debug.Assert(undo != null && redo != null);

            _undoAction = undo;
            _redoAction = redo;
        }

        public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string name)
            : this(
                  () => instance.GetType().GetProperty(property).SetValue(instance, undoValue),
                  () => instance.GetType().GetProperty(property).SetValue(instance, redoValue),
                  name)
        {}
    }

    public class UndoRedo
    {
        private bool _enableAdd = true;

        private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();

        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }
        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }

        public void Reset()
        {
            _undoList.Clear();
            _redoList.Clear();
        }

        public void Add(IUndoRedo cmd)
        {
            if(_enableAdd)
            {
                _undoList.Add(cmd);
                _redoList.Clear();
            }
        }

        public void Undo()
        {
            if(_undoList.Any())
            {
                var undo = _undoList.Last();
                _undoList.RemoveAt(_undoList.Count - 1);
                _enableAdd = false;
                undo.Undo();
                _enableAdd = true;
                _redoList.Insert(0, undo);
            }
        }

        public void Redo()
        {
            if(_redoList.Any())
            {
                var redo = _redoList.First();
                _redoList.RemoveAt(0);
                _enableAdd = false;
                redo.Redo();
                _enableAdd = true;
                _undoList.Add(redo);
            }
        }

        public UndoRedo()
        {
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
        }
    }
}
