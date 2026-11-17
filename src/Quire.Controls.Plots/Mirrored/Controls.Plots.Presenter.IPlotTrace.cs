#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Traces;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotTrace : IPlotTraceStyle, ISymbolProperties, IHasNumberOfPoints
{
    new bool IsNumOfPointsMutable { get; set; }

    Color RenderColor { get; }

    TraceType Type { get; set; }

    double SymbolFrequency { get; set; }

    string TraceSymbol { get; }

    string Label { get; set; }

    IList<Point> PlottingPoints { get; set; }

    bool HasEquationError { get; set; }

    IPlotAxisView XAxis { get; }

    IPlotAxisView YAxis { get; }

    bool IsSelected { get; }

    string TraceLineStyle { get; }

    bool ResultValid { set; }

    EngineError PlotCalculatableError { get; set; }

    SynchronizationStatus SynchronizationStatus { get; set; }

    CalculatingStatus CalculatingStatus { get; set; }

    bool IsCommitted { get; set; }

    double XMinGap { get; }

    double YMinGap { get; }

    bool IsResizing { get; set; }

    PlotGraphAxis.Point2ScreenHelper XPoint2ScreenHelper { get; }

    PlotGraphAxis.Point2ScreenHelper YPoint2ScreenHelper { get; }

    TraceViewModel ViewModel { get; set; }

    Rect ClippingRect { get; }

    Geometry ClippingGeometry { get; }

    bool ForcedToDensityVisualization { get; }

    bool AreDensityPointsSupported { get; }

    bool RestrictedToDensityPointsOrSolidLine { get; }

    BitmapSource CopiedImageBitmap { get; set; }

    Rect ViewPortRect { get; set; }

    event Action IsNumOfPointsMutableChanged;

    event Action NumberOfPointsChanged;

    event Action<IPlotTrace> TraceChanged;

    event Action<IPlotTrace> ErrorChanged;

    event Action CalculatingStatusChangedEvent;

    event Action<IPlotTrace> TraceClicked;

    event Action RequestResetIsTypeDefault;

    void SetEquation(AxesType axis, IPlotEquation equation);

    void SetAxis(IPlotAxisView xAxis, IPlotAxisView yAxis);

    void SelectTrace();

    void DeselectTrace();

    void fireTraceClicked();

    void MoveFocusToEquation(AxesIndex id);

    void ResetIsTypeDefault();
}
