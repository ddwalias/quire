using System;
using System.Collections.Generic;
using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class AxesSetPropertyCommand : IUndoableCommand
{
    private readonly bool _newValue;

    private readonly Action<PlotGraphAxis, bool> _set;

    private readonly Dictionary<PlotGraphAxis, bool> _oldValues;

    public string Name => "Set boolean property on axes";

    public AxesSetPropertyCommand(IEnumerable<PlotGraphAxis> plotGraphAxes, bool newValue, Action<PlotGraphAxis, bool> set, Func<PlotGraphAxis, bool> get)
    {
        _newValue = newValue;
        _set = set;
        _oldValues = new Dictionary<PlotGraphAxis, bool>();
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
        foreach (KeyValuePair<PlotGraphAxis, bool> oldValue in _oldValues)
        {
            _set(oldValue.Key, oldValue.Value);
        }
    }

    public void Redo()
    {
        Do();
    }
}
