#nullable disable
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class NumberFormatSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationNumberFormatSerializerMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreSerializationNumberFormatSerializerMechanicalDifferentialTests")]
    public bool NumberFormatWasRead { get; private set; }
}
