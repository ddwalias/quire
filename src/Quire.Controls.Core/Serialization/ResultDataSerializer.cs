#nullable disable
using System.Xml.Linq;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class ResultDataSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationResultDataSerializerMechanicalDifferentialTests")]
    public static XName GetXNameWithResultNameSpace(string name) => XName.Get(name, "http://schemas.mathsoft.com/result10");
}
