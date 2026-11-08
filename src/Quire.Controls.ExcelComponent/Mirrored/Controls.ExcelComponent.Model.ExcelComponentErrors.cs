#nullable disable
#pragma warning disable

namespace Quire.Controls.ExcelComponent.Model;

public static partial class ExcelComponentErrors
{
    public const string ExcelSheetNotFoundError = "excel_sheet_not_found";
    public const string ExcelInvalidRangeError = "excel_range_address";
    public const string ExcelNotSupportedValueTypeError = "excel_not_supported_value_type";
    public const string NonContiguousRangeError = "non_contiguous_range";
    public const string COMExceptionError = "com_exception";
    public const string ExcelVersionNotInstalledError = "excel_version_not_installed";
}
