#nullable disable
using System.Xml.Linq;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

internal partial class IntegrationElementsSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationIntegrationElementsSerializerMechanicalDifferentialTests")]
    public void RepairXDocument(XDocument xDocument) => Integration10Repairer.RepairXDocument(xDocument);

    [MirrorVerified("PtcControlsCoreSerializationIntegrationElementsSerializerMechanicalDifferentialTests")]
    public bool ShouldValidateOnWrite => true;

    [MirrorVerified("PtcControlsCoreSerializationIntegrationElementsSerializerMechanicalDifferentialTests")]
    public bool ShouldValidateOnRead => true;
}
