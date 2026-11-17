#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.ViewModel;

public partial class TraceViewModel : ViewModelBase
{
    private readonly ITraceModel _model;

    public ITraceImageRendererViewModel ImageRendererViewModel { get; private set; }

    public bool IsUsingDensityVisualization => _model.IsUsingDensityVisualization;

    public PlotType PlotType => _model.PlotType;

    public bool RestrictedToDensityPointsOrSolidLine => _model.RestrictedToDensityPointsOrSolidLine;

    public bool AreDensityPointsSupported => _model.AreDensityPointsSupported;

    public bool ForcedToDensityVisualization => _model.ForcedToDensityVisualization;

    public bool ShouldNotCopyResults
    {
        get
        {
            return _model.ShouldNotCopyFullResults;
        }
        set
        {
            _model.ShouldNotCopyFullResults = value;
        }
    }

    public TraceViewModel(ITraceModel model)
    {
        _model = model;
        _model.PropertyChanged += ModelPropertyChangedInner;
        ImageRendererViewModel = new TraceImageRendererViewModel(_model.ImageRendererModel);
    }

    internal double GetClosestPlottingCoordinateOnRangeAxis(double value, double threshold)
    {
        return _model.GetClosestPlottingCoordinateOnRangeAxis(value, threshold);
    }

    internal List<Point> GetGraphPointsForPlottingCoordinateOnRangeAxis(double rangeAxisValue)
    {
        return _model.GetGraphPointsForPlottingCoordinateOnRangeAxis(rangeAxisValue);
    }
}
