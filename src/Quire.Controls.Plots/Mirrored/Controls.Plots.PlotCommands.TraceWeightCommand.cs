using System.Collections.Generic;
using Quire;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class TraceWeightCommand : TraceCommands
{
    private readonly Dictionary<IPlotTraceStyle, double> _oldWeightMap = new Dictionary<IPlotTraceStyle, double>();

    private readonly Dictionary<IPlotTraceStyle, TraceStyleState?> _exUndoData = new Dictionary<IPlotTraceStyle, TraceStyleState?>();

    private readonly double _newWeight;

    public TraceWeightCommand(IEnumerable<IPlotTraceStyle> traceList, double newWeight)
        : base(traceList, "TraceWeight Command")
    {
        _newWeight = newWeight;
        _traceList.Each((IPlotTraceStyle trace) =>
        {
            _oldWeightMap[trace] = trace.LineWeight;
        });
    }

    public override bool Do()
    {
        _traceList.Each((IPlotTraceStyle trace) =>
        {
            trace.LineWeight = _newWeight;
            _exUndoData.Add(trace, trace.DoStyleChangeCommand());
        });
        return true;
    }

    public override void Undo()
    {
        _traceList.Each((IPlotTraceStyle trace) =>
        {
            trace.LineWeight = _oldWeightMap[trace];
            if (_exUndoData.TryGetValue(trace, out var value))
            {
                trace.UndoStyleChangeCommand(value);
            }
            else
            {
                DebugTools.FailAssertion("A trace exists that Do() wasn't called on!");
            }
        });
        _exUndoData.Clear();
    }

    public override void Redo()
    {
        Do();
    }
}
