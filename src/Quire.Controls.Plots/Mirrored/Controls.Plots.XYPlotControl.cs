#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.View;
using Quire.Wpf;

namespace Quire.Controls.Plots;

public partial class XYPlotControl
{
    private readonly PlotGraphLinearAxis _xAxis;

    private readonly PlotGraphLinearAxis _yAxis;

    public IEnumerable<AxisMarker> VerticalMarkersForSerialization
    {
        get
        {
            return Model.rangeAxis.Markers;
        }
        set
        {
            foreach (AxisMarker item in value)
            {
                Model.rangeAxis.Markers.Add(item);
            }
        }
    }

    public IEnumerable<AxisMarker> HorizontalMarkersForSerialization
    {
        get
        {
            return Model.PrimaryResultAxis.Markers;
        }
        set
        {
            foreach (AxisMarker item in value)
            {
                Model.PrimaryResultAxis.Markers.Add(item);
            }
        }
    }

    public XYPlotControl()
        : base(PlotType.XY, new XYPlotStrategy())
    {
        _xAxis = (PlotGraphLinearAxis)RangeAxis;
        _yAxis = (PlotGraphLinearAxis)PrimaryResultAxis;
        Graph.XAxis = _xAxis;
        Graph.YAxis = _yAxis;
    }

    public XYPlotControl(Size graphSize)
        : this()
    {
        Model.GraphSize = graphSize;
        FixateAxesSizeAtDeserialization();
    }

    protected override void BindCommands()
    {
        base.BindCommands();
        BindChangeOriginPositioningCommand();
    }

    static XYPlotControl()
    {
        PlotControlBase.HasOriginPositioningProperty.OverrideMetadata(typeof(XYPlotControl), new PropertyMetadata(true, PlotControlBase.HasOriginPositioningPropertyChanged));
    }

    public override void EndInit()
    {
        base.EndInit();
        if (Model != null && _xAxis != null && _yAxis != null)
        {
            if (_xAxis.EquationList?.Visibility == Visibility.Visible || _yAxis.EquationList?.Visibility == Visibility.Visible)
            {
                FixateAxesSizeAtDeserialization();
                return;
            }
            WpfUtils.ExecuteOnLoad(this, () =>
            {
                if (Graph.ActualWidth > 0.0 && Graph.ActualHeight > 0.0)
                {
                    FixateAxesSizeAtDeserialization();
                }
            });
        }
    }

    protected override void PostInitializeResizeEventsAfterLoading()
    {
        ReleaseAxisSizes();
        base.PostInitializeResizeEventsAfterLoading();
    }

    private void ReleaseAxisSizes()
    {
        if (_xAxis.SizeFrozen && _yAxis.SizeFrozen)
        {
            _xAxis.ReleaseAutoReSize();
            _yAxis.ReleaseAutoReSize();
        }
    }

    protected override bool SetGraphSize(int numCells)
    {
        if (base.SetGraphSize(numCells))
        {
            return true;
        }
        return FixateAxesSizeAtDeserialization();
    }

    private bool FixateAxesSizeAtDeserialization()
    {
        Size graphSize = Model.GraphSize;
        if (graphSize.Height > 0.0)
        {
            graphSize = Model.GraphSize;
            if (graphSize.Width > 0.0)
            {
                PlotGraphLinearAxis xAxis = _xAxis;
                graphSize = Model.GraphSize;
                xAxis.FixateSize(graphSize.Height);
                PlotGraphLinearAxis yAxis = _yAxis;
                graphSize = Model.GraphSize;
                yAxis.FixateSize(graphSize.Width);
                return true;
            }
        }
        return false;
    }
}
