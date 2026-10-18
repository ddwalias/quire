#nullable disable
using Quire.Mirror;

namespace Quire.Controls.IO;

public partial class ItemIssue
{
    [MirrorVerified("PtcControlsIOItemIssueMechanicalDifferentialTests")]
    public ItemIssue() { }

    [MirrorVerified("PtcControlsIOItemIssueMechanicalDifferentialTests")]
    public string Item { get; set; }

    [MirrorVerified("PtcControlsIOItemIssueMechanicalDifferentialTests")]
    public string Issue { get; set; }
}
