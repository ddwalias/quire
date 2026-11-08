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

    public InputOutputItemsConflicts()
    {
        _generalWarnings = new List<GeneralIssue>();
        _generalErrors = new List<GeneralIssue>();
        _itemWarnings = new List<ItemIssue>();
        _itemErrors = new List<ItemIssue>();
    }

    public void AddGeneralWarning(string warning) => _generalWarnings.Add(new GeneralIssue { Issue = warning });

    public void AddGeneralError(string error) => _generalErrors.Add(new GeneralIssue { Issue = error });

    public void AddItemWarning(string alias, string warning) => _itemWarnings.Add(new ItemIssue { Item = alias, Issue = warning });

    public void AddItemError(string alias, string error) => _itemErrors.Add(new ItemIssue { Item = alias, Issue = error });

    public void AppendLists(IInputOutputItemsConflicts source)
    {
        _generalWarnings.AddRange(source.GeneralWarnings);
        _generalErrors.AddRange(source.GeneralErrors);
        _itemWarnings.AddRange(source.ItemWarnings);
        _itemErrors.AddRange(source.ItemErrors);
    }
}
