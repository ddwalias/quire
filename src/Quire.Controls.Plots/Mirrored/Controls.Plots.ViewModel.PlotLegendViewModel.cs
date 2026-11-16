#nullable disable
#pragma warning disable

using System.ComponentModel;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.ViewModel;

public partial class PlotLegendViewModel
{
    private readonly IPlotLegendModel _model;

    public bool ShowPlotLegend
    {
        get
        {
            return _model.ShowPlotLegend;
        }
        set
        {
            if (_model.ShowPlotLegend == value)
            {
                return;
            }
            _model.ShowPlotLegend = value;
        }
    }

    public LegendPositions.PlotLegendPosition Position
    {
        get
        {
            return _model.Position;
        }
        set
        {
            if (_model.Position == value)
            {
                return;
            }
            _model.Position = value;
        }
    }

    public bool ShowBorder
    {
        get
        {
            return _model.ShowBorder;
        }
        set
        {
            if (_model.ShowBorder == value)
            {
                return;
            }
            _model.ShowBorder = value;
        }
    }

    public double LegendWidth
    {
        get
        {
            return _model.LegendWidth;
        }
        set
        {
            if (_model.LegendWidth.Equals(value))
            {
                return;
            }
            _model.LegendWidth = value;
        }
    }

    public PlotLegendViewModel(IPlotLegendModel plotLegendModel)
    {
        _model = plotLegendModel;
        _model.PropertyChanged += HandleModelPropertyChanged;
    }

    private void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
        case "ShowPlotLegend":
        case "Position":
        case "ShowBorder":
        case "LegendWidth":
            _model.IsDirty = true;
            OnPropertyChanged(e.PropertyName);
            break;
        }
    }
}
