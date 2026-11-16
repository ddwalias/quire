#nullable disable
#pragma warning disable

namespace Quire.Microsoft.Office.Interop.Excel;

public partial interface AppEvents
{
    void WorkbookOpen(Workbook Wb);

    void WorkbookActivate(Workbook Wb);
}
