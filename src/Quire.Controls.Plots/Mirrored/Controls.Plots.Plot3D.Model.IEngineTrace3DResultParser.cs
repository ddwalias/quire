using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Quire.Calculation.Core;

namespace Quire.Controls.Plots.Plot3D.Model;

internal partial interface IEngineTrace3DResultParser
{
    TraceType3D TraceType { get; }

    EngineTraceType3D EngineTraceType { get; }

    Rect3D TraceBounds { get; }

    IMatrixResultNode PointsMatrix { get; }

    bool IsInputRangeFullyConstrained { get; }

    Func<double, double, double, bool> Contains { get; set; }

    IList<Unit> Units { get; }

    EngineError Error { get; }

    PlotTraceVisualData3D CreateVisualData(Transform3D transformForVisual);
}
