#nullable disable
using System.Xml.Serialization;
using System.Xml.Serialization;
using Quire.Mirror;
using Quire.Mirror;
using Quire.Xml;
using Quire.Xml;

namespace Quire.PersistentDataObjects;

public partial class worksheetRegionType
{
    public static XmlSerializer GetXmlSerializer(bool useOverrides)
    {
        if (useOverrides)
        {
            return _xmlSerializerWithOverrides ?? (_xmlSerializerWithOverrides = CreateSerializerWithOverrides(_typeOfThis, "region"));
        }
        return _xmlSerializerDefault ?? (_xmlSerializerDefault = CreateDefaultSerializer(_typeOfThis, "region"));
    }

    [MirrorVerified("PtcPersistentDataObjectsWorksheetRegionTypeSerializerMechanicalDifferentialTests")]
    public static XmlSerializer CreateDefaultSerializer(Type typeOfRoot, string rootNodeName)
    {
        using (new XmlSerializerCreationWrapper())
        {
            return new XmlSerializer(typeOfRoot, new XmlRootAttribute(rootNodeName)
            {
                Namespace = "http://schemas.mathsoft.com/worksheet50"
            });
        }
    }

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

    [MirrorVerified("Release IL")]
    public XmlSerializer XmlSerializer => GetXmlSerializer(base.UseOverrides);
}
