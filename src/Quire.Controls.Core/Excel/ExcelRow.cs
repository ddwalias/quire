#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Excel;

public partial class ExcelRow
{
    [MirrorVerified("PtcControlsCoreExcelExcelRowMechanicalDifferentialTests")]
    public ExcelRow() { }

    [MirrorVerified("PtcControlsCoreExcelExcelRowMechanicalDifferentialTests")]
    public int Row { get; set; }

    [MirrorVerified("PtcControlsCoreExcelExcelRowMechanicalDifferentialTests")]
    public IDictionary<string, ExcelCell> Cells { get; set; }
}
