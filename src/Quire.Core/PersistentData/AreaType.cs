#nullable disable
using System.Xml.Serialization;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class AreaType
{
    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    public XmlSerializer GetXmlSerializer(bool useOverrides) => GetXmlSerializerStatic(useOverrides);

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    private worksheetRegionType[] regionsField;

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    public worksheetRegionType[] regions { get => regionsField; set => regionsField = value; }

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    private ProtectionDataType protectionDataField;

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    public ProtectionDataType ProtectionData { get => protectionDataField; set => protectionDataField = value; }

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    private bool isCollapsedField;

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    public bool IsCollapsed { get => isCollapsedField; set => isCollapsedField = value; }

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    private bool isBorderVisibleField;

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    public bool IsBorderVisible { get => isBorderVisibleField; set => isBorderVisibleField = value; }

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    private double areaHeightField;

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    public double AreaHeight { get => areaHeightField; set => areaHeightField = value; }
}
