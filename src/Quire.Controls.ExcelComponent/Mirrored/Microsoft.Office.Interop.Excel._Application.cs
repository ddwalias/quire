#nullable disable
#pragma warning disable

namespace Quire.Microsoft.Office.Interop.Excel;

public partial interface _Application
{
    Workbook ActiveWorkbook { get; }
    void Calculate();
    object Selection { get; }
    Windows Windows { get; }
    XlCalculation Calculation { get; set; }
    string _Default { get; }
    bool Interactive { get; set; }
    bool UserControl { get; set; }
    bool Visible { get; set; }
}
