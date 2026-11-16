using System;
using System.Collections.Generic;
using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class AxesSetPropertyCommand<T> : IUndoableCommand
{
    private readonly T _newValue;

    private readonly Action<PlotGraphAxis, T> _set;

    private readonly Dictionary<PlotGraphAxis, T> _oldValues;

    private readonly string _name;

    public string Name => _name;

    public AxesSetPropertyCommand(IEnumerable<PlotGraphAxis> plotGraphAxes, T newValue, Action<PlotGraphAxis, T> set, Func<PlotGraphAxis, T> get, string name = null)
    {
        _newValue = newValue;
        _set = set;
        _name = name ?? "Set property on axes";
        _oldValues = new Dictionary<PlotGraphAxis, T>();
        foreach (PlotGraphAxis plotGraphAxis in plotGraphAxes)
        {
            _oldValues.Add(plotGraphAxis, get(plotGraphAxis));
        }
    }

    public bool Do()
    {
        foreach (PlotGraphAxis key in _oldValues.Keys)
        {
            _set(key, _newValue);
        }
        return true;
    }

    public void Undo()
    {
        foreach (KeyValuePair<PlotGraphAxis, T> oldValue in _oldValues)
        {
            _set(oldValue.Key, oldValue.Value);
        }
    }

    public void Redo()
    {
        Do();
    }
}
