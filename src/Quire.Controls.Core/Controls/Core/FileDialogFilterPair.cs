#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial struct FileDialogFilterPair
{
    [MirrorVerified("PtcControlsCoreFileDialogFilterPairMechanicalDifferentialTests")]
    public string FilterString;

    [MirrorVerified("PtcControlsCoreFileDialogFilterPairMechanicalDifferentialTests")]
    public override string ToString() => FilterString;
}
