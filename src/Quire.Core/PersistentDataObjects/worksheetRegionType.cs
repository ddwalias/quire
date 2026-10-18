#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class worksheetRegionType
{
    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    public worksheetRegionType() { }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    public override object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    private userValueType[] metadataField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    public userValueType[] metadata { get => metadataField; set => metadataField = value; }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    private worksheetRegionTypeIndexes indexesField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    public worksheetRegionTypeIndexes indexes { get => indexesField; set => indexesField = value; }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    private worksheetRegionTypeRendering renderingField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    public worksheetRegionTypeRendering rendering { get => renderingField; set => renderingField = value; }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    private string[] mc14conversionlossannotationsField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    public override string[] mc14conversionlossannotations { get => mc14conversionlossannotationsField; set => mc14conversionlossannotationsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeMechanicalDifferentialTests")]
    public InputControlMemberType InputControlMemberType { get; set; }
}
