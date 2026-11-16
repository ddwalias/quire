using System;
using Quire.Controls.Plots.Presenter;
using Quire.PersistentDataObjects;

namespace Quire.PlotDataObjects;

internal static partial class PlotDataObjectCreator
{
    public static traceLineStyle ConvertLineStyle(LineStyle lineStyle)
    {
        switch (lineStyle)
        {
            case LineStyle.None:
                return traceLineStyle.None;
            case LineStyle.Solid:
                return traceLineStyle.Solid;
            case LineStyle.Dot:
                return traceLineStyle.Dot;
            case LineStyle.Dash:
                return traceLineStyle.Dash;
            case LineStyle.DashDot:
                return traceLineStyle.DashDot;
            case LineStyle.DashDotDot:
                return traceLineStyle.DashDotDot;
            default:
                throw new ArgumentOutOfRangeException("lineStyle");
        }
    }

    public static LineStyle ConvertLineStyle(traceLineStyle lineStyle)
    {
        switch (lineStyle)
        {
            case traceLineStyle.None:
                return LineStyle.None;
            case traceLineStyle.Solid:
                return LineStyle.Solid;
            case traceLineStyle.Dot:
                return LineStyle.Dot;
            case traceLineStyle.Dash:
                return LineStyle.Dash;
            case traceLineStyle.DashDot:
                return LineStyle.DashDot;
            case traceLineStyle.DashDotDot:
                return LineStyle.DashDotDot;
            default:
                throw new ArgumentOutOfRangeException("lineStyle");
        }
    }
}
