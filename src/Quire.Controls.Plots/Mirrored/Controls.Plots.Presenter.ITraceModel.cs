#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Plots.Model;

namespace Quire.Controls.Plots.Presenter;

public partial interface ITraceModel : IPlotCalculatable, ICalculatableInfo, INotifyPropertyChanged, ICalculationHideableChild, ICalculatableResultOwner, ICalculatable, ICalculationErrorOwner, IHasNumberOfPoints, IResultOwnerWithSerializableEngineResult
{
    Color Color { get; set; }

    TraceSymbol Symbol { get; set; }

    double SymbolWeight { get; set; }

    double SymbolFrequency { get; set; }

    LineStyle LineStyle { get; set; }

    double LineWeight { get; set; }

    string Label { get; set; }

    TraceType Type { get; set; }

    bool XIsDiscrete { get; }

    bool YIsDiscrete { get; }

    Point TopLeft { get; }

    Point BottomRight { get; }

    XmlNode RegionXmlNode { get; set; }

    bool ResultValid { get; }

    bool ContainsEmptyEquation { get; }

    double? MinPositiveX { get; }

    double? MinPositiveY { get; }

    ITraceImageRendererModel ImageRendererModel { get; }

    bool IsUsingDensityVisualization { get; }

    bool ForcedToDensityVisualization { get; }

    bool AreDensityPointsSupported { get; }

    bool RestrictedToDensityPointsOrSolidLine { get; }

    PlotType PlotType { get; }

    bool ShouldNotCopyFullResults { get; set; }

    event Action PlottingPointsChanged;

    event Action<ITraceModel> AnalyzedDataChanged;

    event Action StyleChanged;

    event Action<bool> IsTraceCommittedChanged;

    event Action<bool> ResultValidChanged;

    event Action ValidateTraceVsPlotType;

    event Action<EngineError> EquationError;

    IAxisModel GetAxis(AxesIndex axis);

    IPlotEquation GetEquation(AxesIndex axis);

    void SetEquation(AxesIndex axis, IEquationItem equationItem);

    PointCollection GetPlottingPointsForDisplay();

    void RecalculateResultOnPlotTypeChange();

    void ResetIsTypeDefault(bool? value = null);

    void ResultAxisLimitValueChanged();

    void RangeAxisLimitValueChanged();

    List<Point> GetGraphPointsForPlottingCoordinateOnRangeAxis(double rangeAxisValue);

    double GetClosestPlottingCoordinateOnRangeAxis(double value, double threshold);
}
