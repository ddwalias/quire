#nullable disable
using Quire.Mirror;

namespace Quire.Controls.IO;

public partial class InputOutputItemsConflicts
{
    [MirrorVerified("PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests")]
    private readonly List<GeneralIssue> _generalWarnings;

    [MirrorVerified("PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests")]
    public IEnumerable<GeneralIssue> GeneralWarnings => _generalWarnings;

    [MirrorVerified("PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests")]
    private readonly List<GeneralIssue> _generalErrors;

    [MirrorVerified("PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests")]
    public IEnumerable<GeneralIssue> GeneralErrors => _generalErrors;

    [MirrorVerified("PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests")]
    private readonly List<ItemIssue> _itemWarnings;

    [MirrorVerified("PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests")]
    public IEnumerable<ItemIssue> ItemWarnings => _itemWarnings;

    [MirrorVerified("PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests")]
    private readonly List<ItemIssue> _itemErrors;

    [MirrorVerified("PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests")]
    public IEnumerable<ItemIssue> ItemErrors => _itemErrors;
}
