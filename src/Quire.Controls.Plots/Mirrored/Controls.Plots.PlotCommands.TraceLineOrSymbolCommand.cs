using System;
using System.Collections.Generic;
using Quire;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.PlotCommands;

internal abstract partial class TraceLineOrSymbolCommand : TraceCommands
{
    private readonly Dictionary<IPlotTraceStyle, TraceStyleState> _previousStyle = new Dictionary<IPlotTraceStyle, TraceStyleState>();

    protected readonly Dictionary<IPlotTraceStyle, TraceStyleState?> _extraUndoData = new Dictionary<IPlotTraceStyle, TraceStyleState?>();

    public TraceLineOrSymbolCommand(IEnumerable<IPlotTraceStyle> traceList, string name)
        : base(traceList, name)
    {
        _traceList.Each((IPlotTraceStyle trace) =>
        {
            _previousStyle.Add(trace, new TraceStyleState(trace.Symbol, trace.LineStyle, trace.LineWeight));
        });
    }

    protected abstract bool DoTrace(IPlotTraceStyle trace);

    public override bool Do()
    {
        bool result = false;
        foreach (IPlotTraceStyle trace in _traceList)
        {
            if (DoTrace(trace))
            {
                result = true;
            }
            _extraUndoData[trace] = trace.DoStyleChangeCommand();
        }
        return result;
    }

    public override void Undo()
    {
        _traceList.Each((IPlotTraceStyle trace) =>
        {
            trace.LineStyle = _previousStyle[trace].LineStyle;
            trace.Symbol = _previousStyle[trace].TraceSymbol;
            trace.LineWeight = _previousStyle[trace].LineWeight;
            if (_extraUndoData.TryGetValue(trace, out var value))
            {
                trace.UndoStyleChangeCommand(value);
            }
            else
            {
                DebugTools.FailAssertion("A trace exists that Do() wasn't called on!");
            }
        });
        _extraUndoData.Clear();
    }

    public override void Redo()
    {
        Do();
    }
}
