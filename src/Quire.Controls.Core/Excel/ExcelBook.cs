#nullable disable
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Controls.Core.Excel;

public partial class ExcelBook
{
    [MirrorVerified("PtcControlsCoreExcelExcelBookMechanicalDifferentialTests")]
    public ExcelBook() { }

    public IEnumerable<ExcelSheet> Sheets;
}
