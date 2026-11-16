using System.Collections.Generic;
using System.Windows.Input;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class GestureManager : IGestureManager
{
    private partial class NullManager : IGestureManager
    {
        public bool Execute(KeyEventArgs args)
        {
            return false;
        }

        public bool Execute(string c)
        {
            return false;
        }

        public void AddBinding(KeyGesture gesture, EditorCommand command)
        {
        }

        public void AddBinding(string key, EditorCommand command)
        {
        }

        public NullManager()
        {
        }
    }

    private static readonly NullManager empty = new NullManager();

    private readonly Dictionary<EditorGesture, EditorCommand> _frame = new Dictionary<EditorGesture, EditorCommand>();

    private readonly IGestureManager _parent;

    public GestureManager()
        : this(empty)
    {
    }

    public GestureManager(IGestureManager _parent)
    {
        this._parent = _parent ?? empty;
    }

    public bool Execute(KeyEventArgs args)
    {
        foreach (KeyValuePair<EditorGesture, EditorCommand> item in _frame)
        {
            if (item.Key.Matches(args))
            {
                item.Value();
                return true;
            }
        }
        return _parent.Execute(args);
    }

    public bool Execute(string c)
    {
        foreach (KeyValuePair<EditorGesture, EditorCommand> item in _frame)
        {
            if (item.Key.Matches(c))
            {
                item.Value();
                return true;
            }
        }
        return _parent.Execute(c);
    }

    public void AddBinding(KeyGesture gesture, EditorCommand command)
    {
        AddBinding(new EditorGesture(gesture), command);
    }

    public void AddBinding(string key, EditorCommand command)
    {
        AddBinding(new EditorGesture(key), command);
    }

    public void AddBinding(EditorGesture gesture, EditorCommand command)
    {
        if (gesture != null && command != null)
        {
            DebugTools.DevAssert(!_frame.ContainsKey(gesture) || _frame[gesture] == command, null);
            _frame[gesture] = command;
        }
        else
        {
            DebugTools.FailAssertion("One of the following is null: gesture: {0}, command: {1}.", gesture, command);
        }
    }
}
