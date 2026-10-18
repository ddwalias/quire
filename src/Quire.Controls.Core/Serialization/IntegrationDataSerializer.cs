#nullable disable
using System.Xml.Linq;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class IntegrationDataSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationIntegrationDataSerializerMechanicalDifferentialTests")]
    public static XName GetXNameWithIntegrationNameSpace(string name) => XName.Get(name, "http://schemas.ptc.com/integration10");
}
