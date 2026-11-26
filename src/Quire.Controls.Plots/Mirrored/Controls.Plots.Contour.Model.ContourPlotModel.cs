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
    private global::Quire.Controls.Plots.Contour.ColorScheme _colorScheme = global::Quire.Controls.Plots.Contour.ColorScheme.Default;

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

    private global::Quire.Controls.Plots.Presenter.LineStyle _lineStyle = global::Quire.Controls.Plots.Presenter.LineStyle.Solid;

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

    private global::System.Windows.Media.Color _contourColor = global::System.Windows.Media.Brushes.Black.Color;

    private bool _showLabels;

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

    private double _lineWeight = 1.0;

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

    private bool _hasOriginPositioning;

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

    public ContourPlotModel(global::Quire.Controls.Plots.Presenter.IAxisModel xAxisModel, global::Quire.Controls.Plots.Presenter.IAxisModel yAxisModel, global::Quire.Controls.Plots.Presenter.IAxisModel zAxisModel)
    {
        XAxisModel = xAxisModel;
        YAxisModel = yAxisModel;
        ZAxisModel = zAxisModel;
        _contourCalculatable = new ContourCalculatable();
        XAxisModel.RangeChanged += XAxisModelRangeChanged;
        YAxisModel.RangeChanged += YAxisModelRangeChanged;
        ZAxisModel.RangeChanged += ZAxisModelRangeChanged;
        _contourCalculatable.PropertyChanged += (sender, e) => ContourCalculatablePropertyChanged(e);
        XAxisModel.Position = global::Quire.Controls.Plots.Presenter.Positions.TickNumberLock;
        YAxisModel.Position = global::Quire.Controls.Plots.Presenter.Positions.TickNumberLock;
        XAxisModel.PositionTickMark = 0;
        YAxisModel.PositionTickMark = 0;
        _contourCalculatable.PlotCalculatableError += ContourCalculatableError;
        SetDefaultLevels();
    }

    private void ContourCalculatableError(object sender, EngineErrorEventArgs e)
    {
        OnPlotCalculatableError(e);
    }

    public override bool SetDataObject(global::Quire.Serialization.IDeS11NProvider de11NProvider, global::Quire.PersistentDataObjects.plotType dataObject, global::Quire.Calculation.Core.IResultManager resultManager) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override global::Quire.PersistentDataObjects.plotType GetDataObject(global::Quire.Serialization.IS11NProvider s11NProvider, global::Quire.Calculation.Core.IResultManager resultManager, bool hideFromCalculationContext) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SetContourDataObject(global::Quire.PersistentDataObjects.contourPlotTypeContour contour, global::Quire.Calculation.Core.IResultManager resultManager) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.PersistentDataObjects.contourPlotTypeContour GetContourDataObject(global::Quire.Calculation.Core.IResultManager resultManager) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private IAxisModel XAxisModel { get; set; }

    private IAxisModel YAxisModel { get; set; }

    private IAxisModel ZAxisModel { get; set; }

    public ICalculatable ContourGraphCalculatable => _contourCalculatable;

    private readonly global::Quire.Controls.Plots.Contour.Model.ContourCalculatable _contourCalculatable;

    private global::Quire.Controls.Plots.Contour.Model.ContourLogicalDataCollection _logicalContoursDataCollection;

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

    private void ZAxisModelRangeChanged()
    {
        if (ZAxisModel.StartExpression != null && ZAxisModel.SecondTickExpression != null && ZAxisModel.EndExpression != null && (ZAxisModel.StartExpression.IsValueValid || ZAxisModel.EndExpression.IsValueValid || ZAxisModel.SecondTickExpression.IsValueValid))
        {
            if (_contourCalculatable.Levels.Array != ZAxisModel.MajorTickMarks.Values)
            {
                _contourCalculatable.Levels = new Levels
                {
                    Array = ZAxisModel.MajorTickMarks.Values
                };
                return;
            }
        }
        if (_contourCalculatable.Levels.Array != null)
        {
            SetDefaultLevels();
        }
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

    public override global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> GetHiddenAndVisibleCalculatables() => base.GetHiddenAndVisibleCalculatables();

    public void InitializeZEquation(IPlotEquation zEquation)
    {
        _contourCalculatable.PlotEquation = zEquation;
    }
}
