#nullable disable
#pragma warning disable

using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Contour.Model;

public sealed partial class ContourPlotModel
{
    public ColorScheme ColorScheme
    {
        get
        {
            return _colorScheme;
        }
        set
        {
            if (_colorScheme != value)
            {
                _colorScheme = value;
                OnPropertyChanged("ColorScheme");
            }
        }
    }

    public LineStyle LineStyle
    {
        get
        {
            return _lineStyle;
        }
        set
        {
            if (_lineStyle != value)
            {
                _lineStyle = value;
                OnPropertyChanged("LineStyle");
            }
        }
    }

    public Color ContoursColor
    {
        get
        {
            return _contourColor;
        }
        set
        {
            if (_contourColor != value)
            {
                _contourColor = value;
                OnPropertyChanged("ContoursColor");
            }
        }
    }

    public bool ShowLabels
    {
        get
        {
            return _showLabels;
        }
        set
        {
            if (_showLabels != value)
            {
                _showLabels = value;
                OnPropertyChanged("ShowLabels");
            }
        }
    }

    public double LineWeight
    {
        get
        {
            return _lineWeight;
        }
        set
        {
            if (LineWeight != value)
            {
                _lineWeight = value;
                OnPropertyChanged("LineWeight");
            }
        }
    }

    public bool HasOriginPositioning
    {
        get
        {
            return _hasOriginPositioning;
        }
        set
        {
            if (_hasOriginPositioning != value)
            {
                _hasOriginPositioning = value;
                OnPropertyChanged("HasOriginPositioning");
            }
        }
    }

    private IAxisModel XAxisModel { get; set; }

    private IAxisModel YAxisModel { get; set; }

    private IAxisModel ZAxisModel { get; set; }

    public ICalculatable ContourGraphCalculatable => _contourCalculatable;

    public ContourLogicalDataCollection LogicalContoursDataCollection
    {
        get
        {
            return _logicalContoursDataCollection;
        }
        private set
        {
            _logicalContoursDataCollection = value;
            UpdateLevels();
            OnPropertyChanged("LogicalContoursDataCollection");
        }
    }
}
