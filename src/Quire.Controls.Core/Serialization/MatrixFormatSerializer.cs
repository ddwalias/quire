#nullable disable
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class MatrixFormatSerializer
{
    [MirrorVerified("PtcControlsCoreSerializationMatrixFormatSerializerMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreSerializationMatrixFormatSerializerMechanicalDifferentialTests")]
    public bool MatrixFormatWasRead { get; private set; }
}
