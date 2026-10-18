#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class FindReplaceQuery
{
    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public FindOptions FindReplaceOption { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public string ValueToFind { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public string ValueToReplaceWith { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public List<Pair<int, int>> FindSubscriptPositions { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public List<Pair<int, int>> ReplaceSubscriptPositions { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public Nullable<FindReplaceStatus> Status { get; set; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public bool IsFindContainSubscript { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public bool FindIsFullySubscript { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public bool IsReplaceContainSubscript { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public bool ReplaceIsFullySubscript { get; }
}
