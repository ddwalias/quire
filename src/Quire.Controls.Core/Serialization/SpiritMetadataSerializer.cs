#nullable disable
using System.Xml;
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

public partial class SpiritMetadataSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationSpiritMetadataSerializerMechanicalDifferentialTests")]
    protected override void CreateSchemas(XmlDocument xmlMetadata) => SpiritResolver.InitializeDocument(xmlMetadata, "OpenPackagingConventions_XMLSchema.opc-coreProperties.xsd");

    [MirrorVerified("PtcControlsCoreSerializationSpiritMetadataSerializerMechanicalDifferentialTests")]
    public SpiritMetadataSerializer() { }
}
