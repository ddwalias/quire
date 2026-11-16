using System;
using System.Collections.Generic;
using Quire;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class TraceSetNumberOfPointsCommand : TraceCommands
{
    private readonly Action _activation;

    private readonly Dictionary<IPlotTraceStyle, int> _oldNumberOfPointsMap = new Dictionary<IPlotTraceStyle, int>();

    private readonly PlotControl.NumberOfPointsAction _action;

    private readonly int _numberOfPointsValue;

    private readonly bool _activate;

    public TraceSetNumberOfPointsCommand(IEnumerable<IPlotTraceStyle> traceList, Pair<int, bool> numberOfPointsData, Action activation, PlotControl.NumberOfPointsAction action = PlotControl.NumberOfPointsAction.Set)
        : base(traceList, "TraceSetNumberOfPoints Command")
    {
        _numberOfPointsValue = numberOfPointsData.First;
        _activate = numberOfPointsData.Second;
        _activation = activation;
        _action = action;
        foreach (IPlotTraceStyle trace in _traceList)
        {
            _oldNumberOfPointsMap[trace] = trace.NumberOfPoints;
        }
    }

    public override bool Do()
    {
        return DoRedoHelper(_activate);
    }

    public override void Undo()
    {
        foreach (IPlotTraceStyle trace in _traceList)
        {
            trace.NumberOfPoints = _oldNumberOfPointsMap[trace];
        }
        _activation();
    }

    public override void Redo()
    {
        DoRedoHelper(true);
    }

    private bool DoRedoHelper(bool activate)
    {
        bool flag = false;
        foreach (IPlotTraceStyle trace in _traceList)
        {
            int num = _oldNumberOfPointsMap[trace];
            switch (_action)
            {
                case PlotControl.NumberOfPointsAction.Increase:
                    num += _numberOfPointsValue;
                    break;
                case PlotControl.NumberOfPointsAction.Decrease:
                    num -= _numberOfPointsValue;
                    break;
                case PlotControl.NumberOfPointsAction.Set:
                    num = _numberOfPointsValue;
                    break;
            }
            int numberOfPoints = trace.NumberOfPoints;
            trace.NumberOfPoints = num;
            flag |= (numberOfPoints != trace.NumberOfPoints);
        }
        if (activate)
        {
            _activation();
        }
        return flag;
    }
}
