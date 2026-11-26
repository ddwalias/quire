#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Model;
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

    private void ContourCalculatableError(object sender, EngineErrorEventArgs e)
    {
        OnPlotCalculatableError(e);
    }

    public void InitializeZEquation(IPlotEquation zEquation)
    {
        _contourCalculatable.PlotEquation = zEquation;
    }

    private void UpdateLevels()
    {
        if (_logicalContoursDataCollection != null && _logicalContoursDataCollection.Levels != null && _logicalContoursDataCollection.Levels.Count > 0)
        {
            if (_contourCalculatable.Levels.Array == null)
            {
                ZAxisModel.SetLevels(_logicalContoursDataCollection.Levels.ToArray());
            }
            else if (_contourCalculatable.Levels.Array.Count() != _logicalContoursDataCollection.Levels.Count)
            {
                _logicalContoursDataCollection.MergeRequestLevels(_contourCalculatable.Levels.Array);
                DebugTools.DevAssert(_contourCalculatable.Levels.Array.Count() == _logicalContoursDataCollection.Levels.Count, null);
            }
        }
    }

    private void YAxisModelRangeChanged()
    {
        double limitValueOrDefault = GetLimitValueOrDefault(YAxisModel.StartExpression, double.NaN);
        double limitValueOrDefault2 = GetLimitValueOrDefault(YAxisModel.EndExpression, double.NaN);
        _contourCalculatable.SetVerticalRange(limitValueOrDefault, limitValueOrDefault2);
    }

    private void XAxisModelRangeChanged()
    {
        double limitValueOrDefault = GetLimitValueOrDefault(XAxisModel.StartExpression, double.NaN);
        double limitValueOrDefault2 = GetLimitValueOrDefault(XAxisModel.EndExpression, double.NaN);
        _contourCalculatable.SetHorizontalRange(limitValueOrDefault, limitValueOrDefault2);
    }

    private static double GetLimitValueOrDefault(IAxisLimitEditor limitExpression, double defaultValue)
    {
        if (limitExpression == null || !limitExpression.IsValueValid)
        {
            return defaultValue;
        }
        return limitExpression.CalculatedValue;
    }

    private void SetDefaultLevels()
    {
        _contourCalculatable.Levels = new Levels
        {
            Number = ZAxisModel.DefaultTickMarks
        };
    }

    private void ContourCalculatablePropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "Result":
                LogicalContoursDataCollection = _contourCalculatable.Result.Convert();
                break;
            case "CalculatingStatus":
                IsCalculating = _contourCalculatable.CalculatingStatus == CalculatingStatus.Calculating;
                break;
            case "SynchronizationStatus":
                OnPropertyChanged("SynchronizationStatus");
                break;
            case "Error":
                HasError = _contourCalculatable.Error != null;
                break;
        }
    }
}
