#nullable disable
using Quire.PersistentData;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class solveBlockRegionType
{
    [MirrorVerified("Release IL")]
    public solveBlockRegionType(worksheetRegionType regionType)
    {
        Item = regionType.Item;
        if (Item is IRegionCollectionType { Regions: not null } regionCollectionType)
        {
            foreach (IRegionType region in regionCollectionType.Regions)
            {
                region.S11NProvider = this;
            }
        }
        convertedfrom = regionType.convertedfrom;
        convertedfromSpecified = regionType.convertedfromSpecified;
        mc14conversionlossannotations = regionType.mc14conversionlossannotations;
        height = regionType.height;
        heightSpecified = regionType.heightSpecified;
        width = regionType.width;
        widthSpecified = regionType.widthSpecified;
        actualHeight = regionType.actualHeight;
        actualHeightSpecified = regionType.actualHeightSpecified;
        actualWidth = regionType.actualWidth;
        actualWidthSpecified = regionType.actualWidthSpecified;
        if (regionType.Tag.IsNotEmpty())
        {
            Tag = regionType.Tag;
        }
        ShowBorder = regionType.ShowBorder;
    }

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
