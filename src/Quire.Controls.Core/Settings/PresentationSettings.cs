#nullable disable
using System.Xml.Linq;
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class PresentationSettings
{
    [MirrorVerified("PtcControlsCoreSettingsPresentationSettingsMechanicalDifferentialTests")]
    protected readonly string _namespaceName;

    [MirrorVerified("PtcControlsCoreSettingsPresentationSettingsMechanicalDifferentialTests")]
    private XName GetXName(string name) => XName.Get(name, _namespaceName);

    [MirrorVerified("PtcControlsCoreSettingsPresentationSettingsMechanicalDifferentialTests")]
    protected static void OnSchemaValidation(object sender, ValidationEventArgs e) { }

    [MirrorVerified("PtcControlsCoreSettingsPresentationSettingsMechanicalDifferentialTests")]
    public void RepairXDocument(XDocument xDocument) => Presentation10Repairer.RepairXDocument(xDocument);

    [MirrorVerified("PtcControlsCoreSettingsPresentationSettingsMechanicalDifferentialTests")]
    protected internal XName PageModelName => GetXName("pageModel");

    [MirrorVerified("PtcControlsCoreSettingsPresentationSettingsMechanicalDifferentialTests")]
    private XName MathRenderingName => GetXName("mathRendering");

    [MirrorVerified("PtcControlsCoreSettingsPresentationSettingsMechanicalDifferentialTests")]
    public bool ShouldValidateOnWrite => true;

    [MirrorVerified("PtcControlsCoreSettingsPresentationSettingsMechanicalDifferentialTests")]
    public bool ShouldValidateOnRead => true;
}
