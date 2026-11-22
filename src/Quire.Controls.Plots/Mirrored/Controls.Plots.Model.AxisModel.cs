#nullable disable
#pragma warning disable

using System.Collections.Generic;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

internal partial class AxisModel
{
    public IPlotTitleModel AxisTitleModel { get; set; }

    public AxisMarkersCollection Markers => _markers;

    public IEnumerable<IEquationItem> Equations => _equations;

    public bool CanHaveMarkers => !IsDiscrete;

    public bool XIsString { get; set; }

    public bool YIsString { get; set; }

    public bool IsDiscrete { get; set; }

    public double LegendWidth { get; set; }

    public string AxisXmlNodeName
    {
        get
        {
            switch (Type)
            {
                case AxesType.X:
                    return "xAxis";
                case AxesType.Y:
                    return "yAxis";
                case AxesType.Z:
                    return "zAxis";
                case AxesType.Angular:
                    return "angularAxis";
                case AxesType.Radial:
                    return "radialAxis";
                default:
                    return "";
            }
        }
    }

    public string DomainXmlNodeName
    {
        get
        {
            switch (Type)
            {
                case AxesType.X:
                    return "xyDomain";
                case AxesType.Y:
                    return "xyDomain";
                case AxesType.Angular:
                    return "angularDomain";
                case AxesType.Radial:
                    return "radialDomain";
                case AxesType.Z:
                    return "zDomain";
                default:
                    return "";
            }
        }
    }

    public AxesType Type { get; private set; }

    public PlotType PlotType { get; private set; }

    public Positions Position { get; set; }

    public int PositionTickMark { get; set; }

    public int LegendPositionIndex { get; set; }

    public bool UseBetterTickMarks { get; set; }

    public double StartDefault { get; private set; }

    public double EndDefault { get; private set; }

    public int DefaultTickMarks { get; private set; }

    public int MaxTickmarks { get; private set; }

    public bool IsDisplayNumberFormatLocal { get; private set; }

    public TickScale MajorTickMarks { get; set; }

    public TickScale MinorTickMarks { get; set; }

    public string[] FilteredLabels { get; private set; }

    public bool ShowTicks
    {
        get => MajorTickMarks.Show;
        set
        {
            if (ShowTicks != value)
            {
                MajorTickMarks.Show = value;
                OnPropertyChanged("ShowTicks");
            }
        }
    }

    public bool ShowGridLines
    {
        get => _showGridLines;
        set
        {
            if (_showGridLines != value)
            {
                _showGridLines = value;
                OnPropertyChanged("ShowGridLines");
            }
        }
    }

    public bool ShowNumbers
    {
        get => _showNumbers;
        set
        {
            if (_showNumbers != value)
            {
                _showNumbers = value;
                OnPropertyChanged("ShowNumbers");
            }
        }
    }
}
