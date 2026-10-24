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

    public static XmlSerializer CreateSerializerWithOverrides(Type typeOfRoot, string rootNodeName)
    {
        XmlAttributeOverrides overrides = new XmlAttributeOverrides();
        Type[] typesOf = new Type[4]
        {
            typeof(pictureType),
            typeof(xyPlotType),
            typeof(polarPlotType),
            typeof(plotBaseType)
        };
        regionBase.GetOverrides(overrides, typesOf);
        plotType.GetOverrides(overrides);
        XmlSerializer serializer;
        using (new XmlSerializerCreationWrapper())
        {
            serializer = new XmlSerializer(typeOfRoot, overrides, new Type[0], new XmlRootAttribute(rootNodeName)
            {
                Namespace = "http://schemas.mathsoft.com/worksheet50"
            }, "http://schemas.mathsoft.com/worksheet50");
        }
        return serializer;
    }

    private bool Serialize(System.Xml.XmlNode regionXmlNode)
    {
        using (XmlNodeWriter xmlWriter = new XmlNodeWriter(regionXmlNode))
        {
            try
            {
                XmlSerializer.Serialize(xmlWriter, this);
            }
            catch (System.Exception ex)
            {
                DebugTools.DevAssert(false, string.Format("region node serialization failed: {0}", ex.Message), Array.Empty<object>());
            }
        }
        return true;
    }

    public System.Xml.XmlElement Serialize()
    {
        PreSerialize();
        System.Xml.XmlElement xmlElement = XmlContentDocument.CreateElement("dummy", "http://schemas.mathsoft.com/worksheet50");
        if (Serialize(xmlElement) && xmlElement.ChildNodes.Count == 1)
        {
            return xmlElement.ChildNodes[0] as System.Xml.XmlElement;
        }
        return null;
    }

    public static worksheetRegionType Deserialize(System.Xml.XmlElement xmlRoot, bool useOverrides)
    {
        object obj;
        using (System.Xml.XmlNodeReader xmlReader = new System.Xml.XmlNodeReader(xmlRoot))
        {
            obj = GetXmlSerializer(useOverrides).Deserialize(xmlReader);
        }
        return obj as worksheetRegionType;
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
