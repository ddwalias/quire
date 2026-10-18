#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MatrixFormatWithKey
{
    [MirrorVerified("PtcControlsCoreMatrixFormatWithKeyMechanicalDifferentialTests")]
    public MatrixFormatWithKey() { }

    [MirrorVerified("PtcControlsCoreMatrixFormatWithKeyMechanicalDifferentialTests")]
    public TableCoordinate Key { get; set; }

    [MirrorVerified("PtcControlsCoreMatrixFormatWithKeyMechanicalDifferentialTests")]
    public MatrixFormat Value { get; set; }
}
