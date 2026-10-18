#nullable disable
using System.ComponentModel;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MatrixFormat
{
    [MirrorVerified("PtcControlsCoreMatrixFormatMechanicalDifferentialTests")]
    public event ActionByPropertyNameAndValue FormatChanged;

    [MirrorVerified("PtcControlsCoreMatrixFormatMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
