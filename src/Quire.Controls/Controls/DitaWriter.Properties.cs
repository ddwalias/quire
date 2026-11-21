#nullable disable
using System.Xml;
using Quire.Mirror;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls;

// Compile-order device: partial declaration separates properties so WriteRegionML matches closure ordinal 30_0
public partial class DitaWriter
{
    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public ISerializationHelper Helper => _helper;

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public XmlNamespaceManager NameSpaceManager => XmlWorksheetData.McdxNameSpaceManager;

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public XmlDocument XmlContentDocument { get; set; }

    [MirrorVerified("PtcControlsDitaWriterMechanicalDifferentialTests")]
    public bool UseOverrides => true;
}
