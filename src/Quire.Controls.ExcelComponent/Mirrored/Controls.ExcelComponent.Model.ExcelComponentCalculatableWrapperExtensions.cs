#nullable disable
#pragma warning disable

namespace Quire.Controls.ExcelComponent.Model;

internal static partial class ExcelComponentCalculatableWrapperExtensions
{
    public static bool IsValidType(this ExcelComponentEquationType type)
    {
        return type != ExcelComponentEquationType.Unknown && type != ExcelComponentEquationType.Invalid;
    }
}
