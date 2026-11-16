using System.Collections.Generic;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Plots.Presenter;
using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class TraceTypeCommand : TraceCommands
{
    private readonly Dictionary<IPlotTrace, TraceType> _oldTraceTypeMap = new Dictionary<IPlotTrace, TraceType>();

    private readonly TraceType _traceType;

    private CompoundCommand _compoundCommand;

    public TraceTypeCommand(IEnumerable<IPlotTrace> traceList, TraceType traceType)
        : base(traceList, "TraceType Command")
    {
        _traceType = traceType;
        foreach (IPlotTrace trace in _traceList)
        {
            _oldTraceTypeMap[trace] = trace.Type;
        }
    }

    public override bool Do()
    {
        ChangeType();
        bool flag = ChangeSymbolIfNeeded();
        if (!flag)
        {
            Undo();
        }
        return flag;
    }

    private void ChangeType()
    {
        foreach (IPlotTrace trace in _traceList)
        {
            trace.Type = _traceType;
        }
    }

    private bool ChangeSymbolIfNeeded()
    {
        CreateChangeSymbolCommand(TraceType.Stems, TraceSymbol.circle);
        CreateChangeSymbolCommand(TraceType.Errors, TraceSymbol.none);
        if (_compoundCommand != null)
        {
            bool flag = _compoundCommand.Do();
            if (!flag)
            {
                _compoundCommand = null;
            }
            return flag;
        }
        return true;
    }

    private void CreateChangeSymbolCommand(TraceType traceType, TraceSymbol traceSymbol)
    {
        List<IPlotTrace> list = new List<IPlotTrace>();
        foreach (IPlotTrace trace in _traceList)
        {
            if (trace.Type == traceType && trace.Symbol != traceSymbol)
            {
                list.Add(trace);
            }
        }
        if (list.Count > 0)
        {
            if (_compoundCommand == null)
            {
                _compoundCommand = new CompoundCommand();
            }
            _compoundCommand.Add(new TraceSymbolCommand(list, traceSymbol));
        }
    }

    public override void Undo()
    {
        foreach (IPlotTrace trace in _traceList)
        {
            trace.Type = _oldTraceTypeMap[trace];
        }
        if (_compoundCommand != null)
        {
            _compoundCommand.Undo();
        }
    }

    public override void Redo()
    {
        Do();
    }
}
