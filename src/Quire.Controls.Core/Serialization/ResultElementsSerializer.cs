#nullable disable
using System.Xml.Linq;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

internal partial class ResultElementsSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationResultElementsSerializerMechanicalDifferentialTests")]
    public void RepairXDocument(XDocument xDocument) => Result10Repairer.RepairXDocument(xDocument);

    [MirrorVerified("PtcControlsCoreSerializationResultElementsSerializerMechanicalDifferentialTests")]
    public bool ShouldValidateOnWrite => true;

    [MirrorVerified("PtcControlsCoreSerializationResultElementsSerializerMechanicalDifferentialTests")]
    public bool ShouldValidateOnRead => false;
}
