#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Excel;

public partial class ExcelCell
{
    [MirrorVerified("PtcControlsCoreExcelExcelCellMechanicalDifferentialTests")]
    public ExcelCell() { }

    [MirrorVerified("PtcControlsCoreExcelExcelCellMechanicalDifferentialTests")]
    public int Row { get; set; }

    [MirrorVerified("PtcControlsCoreExcelExcelCellMechanicalDifferentialTests")]
    public string Column { get; set; }

    [MirrorVerified("PtcControlsCoreExcelExcelCellMechanicalDifferentialTests")]
    public string Value { get; set; }
}
