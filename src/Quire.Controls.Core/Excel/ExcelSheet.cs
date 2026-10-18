#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Excel;

public partial class ExcelSheet
{
    [MirrorVerified("PtcControlsCoreExcelExcelSheetMechanicalDifferentialTests")]
    public ExcelSheet() { }

    [MirrorVerified("PtcControlsCoreExcelExcelSheetMechanicalDifferentialTests")]
    public string Name { get; set; }

    [MirrorVerified("PtcControlsCoreExcelExcelSheetMechanicalDifferentialTests")]
    public IEnumerable<ExcelRow> Rows { get; set; }

    [MirrorVerified("PtcControlsCoreExcelExcelSheetMechanicalDifferentialTests")]
    public IEnumerable<string> Columns { get; set; }
}
