#nullable disable
#pragma warning disable

namespace Quire.Microsoft.Office.Interop.Excel;

public partial interface _Workbook
{
    Application Application { get; }
    object ActiveSheet { get; }
    void Close(object SaveChanges, object Filename, object RouteWorkbook);
    string Name { get; }
    Windows Windows { get; }

    Sheets Worksheets { get; }
}
