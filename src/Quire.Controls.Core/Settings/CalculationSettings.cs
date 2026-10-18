#nullable disable
using System.Xml.Linq;
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class CalculationSettings
{
    [MirrorVerified("PtcControlsCoreSettingsCalculationSettingsMechanicalDifferentialTests")]
    protected static void OnSchemaValidation(object sender, ValidationEventArgs e) { }

    [MirrorVerified("PtcControlsCoreSettingsCalculationSettingsMechanicalDifferentialTests")]
    public void RepairXDocument(XDocument xDocument) => Calculation10Repairer.RepairXDocument(xDocument);

    [MirrorVerified("PtcControlsCoreSettingsCalculationSettingsMechanicalDifferentialTests")]
    public bool ShouldValidateOnWrite => true;

    [MirrorVerified("PtcControlsCoreSettingsCalculationSettingsMechanicalDifferentialTests")]
    public bool ShouldValidateOnRead => true;
}
