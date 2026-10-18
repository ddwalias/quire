#nullable disable
using System.IO.Packaging;
using System.Xml;
using Quire.Mirror;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

public partial class McdxSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    private readonly ITextStyleChangedHandler _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler => _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    public XmlDocument XmlContentDocument { get; private set; }

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    public bool UseOverrides { get; private set; }

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    private readonly PackagePart _packagePart;

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    public PackagePart PackagePart => _packagePart;

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    private readonly ISerializationHelper _serializationHelper;

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    public ISerializationHelper Helper => _serializationHelper;

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    public XmlNamespaceManager NameSpaceManager => XmlWorksheetData.McdxNameSpaceManager;
}
