#nullable disable
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ResultFormat
{
    [MirrorVerified("PtcControlsCoreResultFormatMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreResultFormatMechanicalDifferentialTests")]
    public bool NumberFormatWasRead { get; private set; }

    [MirrorVerified("PtcControlsCoreResultFormatMechanicalDifferentialTests")]
    public bool MatrixFormatWasRead { get; private set; }

    [MirrorVerified("PtcControlsCoreResultFormatMechanicalDifferentialTests")]
    public string RootNodeFormatName { get; set; }
}
