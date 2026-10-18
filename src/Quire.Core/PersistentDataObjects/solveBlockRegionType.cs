#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class solveBlockRegionType
{
    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    public solveBlockRegionType() { }

    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    public override object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    private string[] mc14conversionlossannotationsField;

    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    public override string[] mc14conversionlossannotations { get => mc14conversionlossannotationsField; set => mc14conversionlossannotationsField = value; }

    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    private solveBlockRegionTypeSolveblockcategory solveblockcategoryField;

    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    public solveBlockRegionTypeSolveblockcategory solveblockcategory { get => solveblockcategoryField; set => solveblockcategoryField = value; }

    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    private bool solveblockcategoryFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectssolveBlockRegionTypeMechanicalDifferentialTests")]
    public bool solveblockcategorySpecified { get => solveblockcategoryFieldSpecified; set => solveblockcategoryFieldSpecified = value; }
}
