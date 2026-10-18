#nullable disable
using System.IO.Packaging;
using System.Xml;
using Quire.Mirror;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

public partial class McdxDeserializer
{
    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private readonly ITextStyleChangedHandler _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler => _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> DeserializedRegions { get; private set; }

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public bool SkipRegionsNotAffectingCalculation { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private readonly PackagePart _packagePart;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public PackagePart PackagePart => _packagePart;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private readonly ISerializationHelper _serializationHelper;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public ISerializationHelper Helper => _serializationHelper;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public XmlNamespaceManager NameSpaceManager => XmlWorksheetData.McdxNameSpaceManager;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private XmlDocument _xmlContentDocument;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public XmlDocument XmlContentDocument => _xmlContentDocument;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public bool UseOverrides { get; private set; }

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private readonly string _filePath;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public string FilePath => _filePath;
}
