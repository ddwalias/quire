#nullable disable
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class ResultsSerializer
{
    [MirrorVerified("PtcControlsCoreSettingsResultsSerializerMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();
}
