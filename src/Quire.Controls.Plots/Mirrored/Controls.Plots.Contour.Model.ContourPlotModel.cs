#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Xml;
using Quire.Calculation.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;
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

    public override bool SetDataObject(IDeS11NProvider de11NProvider, plotType dataObject, IResultManager resultManager)
    {
        IsDeserializing = true;
        contourPlotType contourPlot = dataObject.Item as contourPlotType;
        DebugTools.DevAssert(contourPlot != null, null, Array.Empty<object>());
        HasOriginPositioning = dataObject.originpositioning;
        ShowAxisExpressions = dataObject.showaxisexpressions;
        PlotBackgroundType = (Quire.Controls.Core.BackgroundTypeE)Enum.Parse(typeof(global::Quire.PersistentDataObjects.BackgroundType), dataObject.backgroundtype.ToString(), true);
        axes3Type axes = contourPlot.axes;
        XAxisModel.RegionXmlNode = axes.xAxisRaw;
        YAxisModel.RegionXmlNode = axes.yAxisRaw;
        XmlNode zAxisRaw = axes.zAxisRaw;
        if (zAxisRaw != null)
        {
            ZAxisModel.RegionXmlNode = zAxisRaw;
        }
        _contourCalculatable.PlotEquation.SetRegionXmlNode(contourPlot.plotEquationRaw);
        SetContourDataObject(contourPlot.contour, resultManager);
        GraphSizeDataObject = contourPlot.graphsize;
        IsDeserializing = false;
        return dataObject.disablecalc;
    }

    public override plotType GetDataObject(IS11NProvider s11NProvider, IResultManager resultManager, bool hideFromCalculationContext)
    {
        axes3Type axes = new axes3Type
        {
            xAxisRaw = XAxisModel.RegionXmlNode,
            yAxisRaw = YAxisModel.RegionXmlNode,
            zAxisRaw = ZAxisModel.RegionXmlNode
        };
        SizeType graphSize = GraphSizeDataObject;
        contourPlotType contourPlot = new contourPlotType
        {
            axes = axes,
            contour = GetContourDataObject(resultManager),
            plotEquationRaw = _contourCalculatable.PlotEquation.GetRegionXmlNode(),
            graphsize = graphSize
        };
        return new plotType
        {
            Item = contourPlot,
            originpositioning = HasOriginPositioning,
            showaxisexpressions = ShowAxisExpressions,
            backgroundtype = (BackgroundType)Enum.Parse(typeof(Quire.Controls.Core.BackgroundTypeE), PlotBackgroundType.ToString(), true),
            disablecalc = hideFromCalculationContext
        };
    }

    public void SetContourDataObject(global::Quire.PersistentDataObjects.contourPlotTypeContour contour, global::Quire.Calculation.Core.IResultManager resultManager) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.PersistentDataObjects.contourPlotTypeContour GetContourDataObject(global::Quire.Calculation.Core.IResultManager resultManager)
    {
        int resultRef = resultManager == null ? -1 : resultManager.CreateResultRefId(_contourCalculatable);
        return new contourPlotTypeContour
        {
            colorscheme = (colorSchemeType)Enum.Parse(typeof(colorSchemeType), ColorScheme.ToString(), true),
            linestyle = (traceLineStyle)Enum.Parse(typeof(LineStyle), LineStyle.ToString(), true),
            color = ContoursColor.ToString(CultureInfo.InvariantCulture),
            showlables = ShowLabels,
            lineweight = LineWeight,
            resultRef = resultRef,
            resultRefSpecified = resultRef != -1
        };
    }

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
