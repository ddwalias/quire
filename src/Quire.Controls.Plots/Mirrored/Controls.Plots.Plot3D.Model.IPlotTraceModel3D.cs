#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.Traces;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots.Plot3D.Model;

public partial interface IPlotTraceModel3D : IPlotCalculatable, ICalculatableInfo, INotifyPropertyChanged, ICalculationHideableChild, ICalculatableResultOwner, ICalculatable, ICalculationErrorOwner, IHasNumberOfPoints, IPlotTraceStyle, ISymbolProperties, IResultOwnerWithSerializableEngineResult
{
    PlotTraceVisualData3D VisualData { get; }

    Rect3D CommonRange { get; set; }

    Rect3D TraceBounds { get; }

    bool AxesHaveMinMaxErrors { set; }

    IPlotEquationModel3D TraceEquation { get; }

    bool IsStyleAutomatic { get; }

    Color? SurfaceFill { get; set; }

    TraceType3D VisualDataTypeOrDefault { get; }

    Color RenderColor { get; }

    bool IsInPrinting { get; set; }

    event Action Changed;

    trace3DType GetDataObject(IResultManager resultManager);

    void SetDataObject(trace3DType traceData, IResultManager instance);

    void SetUserDefinedLimits(double xStart, double xEnd, double yStart, double yEnd, double zStart, double zEnd, out bool needsCommit);

    void ResetOnRemoval();

    void UserChangedStyle();
}
