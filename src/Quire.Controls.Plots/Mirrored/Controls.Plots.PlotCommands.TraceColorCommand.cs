using System.Collections.Generic;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class TraceColorCommand : TraceCommands
{
    private readonly Dictionary<IPlotTraceStyle, Color> _oldColorMap = new Dictionary<IPlotTraceStyle, Color>();

    private readonly Color _newColor;

    public TraceColorCommand(IEnumerable<IPlotTraceStyle> traceList, Color newColor)
        : base(traceList, "TraceColor Command")
    {
        _newColor = newColor;
        foreach (IPlotTraceStyle trace in _traceList)
        {
            _oldColorMap[trace] = trace.Color;
        }
    }

    public override bool Do()
    {
        foreach (IPlotTraceStyle trace in _traceList)
        {
            trace.Color = _newColor;
        }
        return true;
    }

    public override void Undo()
    {
        foreach (IPlotTraceStyle trace in _traceList)
        {
            trace.Color = _oldColorMap[trace];
        }
    }

    public override void Redo()
    {
        Do();
    }
}
