using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Quire;
using Quire.Controls.Plots.Plot3D.ViewModel;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class SurfaceFillCommand : TraceCommands
{
    private readonly Dictionary<IPlotTraceViewModel3D, Color?> _oldValues = new Dictionary<IPlotTraceViewModel3D, Color?>();

    private readonly Color? _newValue;

    private IEnumerable<IPlotTraceViewModel3D> Traces3D => _traceList.Cast<IPlotTraceViewModel3D>();

    public SurfaceFillCommand(IEnumerable<IPlotTraceViewModel3D> traceList, Color? newColor)
        : base(traceList, "SurfaceFillCommand")
    {
        _newValue = newColor;
        Traces3D.Each((IPlotTraceViewModel3D tr) =>
        {
            _oldValues.Add(tr, tr.SurfaceFill);
        });
    }

    public override bool Do()
    {
        Traces3D.Each((IPlotTraceViewModel3D tr) =>
        {
            tr.SurfaceFill = _newValue;
        });
        return true;
    }

    public override void Undo()
    {
        Traces3D.Each((IPlotTraceViewModel3D tr) =>
        {
            tr.SurfaceFill = _oldValues[tr];
        });
    }

    public override void Redo()
    {
        Do();
    }
}
