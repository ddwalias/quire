namespace Quire.Controls.ExcelComponent.Model;

internal static partial class ChartComponentCalculatableWrapperExtensions
{
    public static bool IsLimitExpression(this global::Quire.Controls.ExcelComponent.Model.IChartComponentCalculatableWrapper wrapper)
    {
        return wrapper.ChartWrapperType.IsLimitType();
    }

    public static bool IsTraceOrCommonExpression(this global::Quire.Controls.ExcelComponent.Model.IChartComponentCalculatableWrapper wrapper)
    {
        return wrapper.ChartWrapperType.IsTraceOrCommonType();
    }

    public static bool IsValidType(this global::Quire.Controls.ExcelComponent.Model.ChartWrapperType type)
    {
        if (type != ChartWrapperType.Unknown)
        {
            return type != ChartWrapperType.Invalid;
        }
        return false;
    }

    public static bool IsUnknownType(this global::Quire.Controls.ExcelComponent.Model.ChartWrapperType type)
    {
        return type == ChartWrapperType.Unknown;
    }

    public static bool IsValidOrUnknownType(this global::Quire.Controls.ExcelComponent.Model.ChartWrapperType type)
    {
        if (!type.IsValidType())
        {
            return type.IsUnknownType();
        }
        return true;
    }

    public static bool IsTraceOrCommonType(this global::Quire.Controls.ExcelComponent.Model.ChartWrapperType type)
    {
        if (!type.IsCommonTraceType())
        {
            return type.IsIndexedTraceType();
        }
        return true;
    }

    public static bool IsCommonTraceType(this global::Quire.Controls.ExcelComponent.Model.ChartWrapperType type)
    {
        if (type != ChartWrapperType.XCommon && type != ChartWrapperType.YCommon)
        {
            return type == ChartWrapperType.ZCommon;
        }
        return true;
    }

    public static bool IsIndexedTraceType(this global::Quire.Controls.ExcelComponent.Model.ChartWrapperType type)
    {
        if (!ChartComponentConstants.TypeToIdentifier.TryGetValue(type, out var value))
        {
            return false;
        }
        return value.Second;
    }

    public static bool IsLimitType(this global::Quire.Controls.ExcelComponent.Model.ChartWrapperType type)
    {
        if (type != ChartWrapperType.XMin && type != ChartWrapperType.XMax && type != ChartWrapperType.YMin && type != ChartWrapperType.YMax && type != ChartWrapperType.Y2Min && type != ChartWrapperType.Y2Max && type != ChartWrapperType.ZMin)
        {
            return type == ChartWrapperType.ZMax;
        }
        return true;
    }
}
