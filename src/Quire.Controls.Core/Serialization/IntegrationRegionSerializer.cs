#nullable disable
using System.Xml.Linq;
using Quire.Controls.Core.IORegions;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

internal partial class IntegrationRegionSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationIntegrationRegionSerializerMechanicalDifferentialTests")]
    private readonly IIORegionContainer _documentArea;

    [MirrorVerified("PtcControlsCoreSerializationIntegrationRegionSerializerMechanicalDifferentialTests")]
    public void WriteXml(XElement xElement) => WriteIORegionsFromContainer(xElement, _documentArea);
}
