#nullable disable
using Quire.Mirror;

namespace Quire.Controls.IO;

public partial class GeneralIssue
{
    [MirrorVerified("PtcControlsIOGeneralIssueMechanicalDifferentialTests")]
    public GeneralIssue() { }

    [MirrorVerified("PtcControlsIOGeneralIssueMechanicalDifferentialTests")]
    public string Issue { get; set; }
}
