#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Excel;

public partial class ExcelReader
{
    [MirrorVerified("PtcControlsCoreExcelExcelReaderMechanicalDifferentialTests")]
    public ExcelBook WorkBook { get; private set; }
}
