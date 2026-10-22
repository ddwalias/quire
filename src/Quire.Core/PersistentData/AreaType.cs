#nullable disable
using System.Linq;
using System.Linq;
using System.Xml.Serialization;
using System.Xml.Serialization;
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class AreaType
{
    public static readonly Type TypeOfThis;
    public bool Protect
    {
        get
        {
            if (ProtectionDataWrapper != null)
            {
                return ProtectionDataWrapper.HasPassword;
            }
            return false;
        }
    }

    [XmlIgnore]
    public override IEnumerable<IRegionType> Regions
    {
        get
        {
            return regions;
        }
        set
        {
            regions = value.Cast<worksheetRegionType>().ToArray();
        }
    }

    [XmlIgnore]
    public ProtectionData ProtectionDataWrapper
    {
        get
        {
            if (ProtectionData != null)
            {
                return new ProtectionData(ProtectionData);
            }
            return null;
        }
        set
        {
            ProtectionData = value?.GetProtectionPersistentData();
        }
    }

    static AreaType()
    {
        TypeOfThis = typeof(AreaType);
    }

    [MirrorVerified("PtcPersistentDataAreaTypeMechanicalDifferentialTests")]
    public XmlSerializer GetXmlSerializer(bool useOverrides) => GetXmlSerializerStatic(useOverrides);

    [MirrorVerified("Release IL")]
    private static XmlSerializer GetXmlSerializerStatic(bool useOverrides)
    {
        if (useOverrides)
        {
            return _xmlSerializerWithOverrides ?? (_xmlSerializerWithOverrides = worksheetRegionType.CreateSerializerWithOverrides(TypeOfThis, XmlRootElementName));
        }
        return _xmlSerializerDefault ?? (_xmlSerializerDefault = worksheetRegionType.CreateDefaultSerializer(TypeOfThis, XmlRootElementName));
    }

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

    [MirrorVerified("Release IL")]
    public AreaType()
    {
        isCollapsedField = false;
        isBorderVisibleField = true;
        areaHeightField = 0.0;
    }
}
