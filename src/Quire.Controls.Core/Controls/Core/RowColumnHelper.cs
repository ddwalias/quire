#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RowColumnHelper
{
    [MirrorVerified("PtcControlsCoreRowColumnHelperMechanicalDifferentialTests")]
    private int _columnCount;

    [MirrorVerified("PtcControlsCoreRowColumnHelperMechanicalDifferentialTests")]
    public RowColumnHelper(int columnCount)
    {
        _columnCount = columnCount;
    }

    [MirrorVerified("PtcControlsCoreRowColumnHelperMechanicalDifferentialTests")]
    public int ColumnCount => _columnCount;
}
