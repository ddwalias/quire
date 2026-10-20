#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.ThirdParty;

public partial class ArgumentAttribute
{
    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    private ArgumentType type;

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    public ArgumentType Type => type;

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    private string shortName;

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    public string ShortName { get => shortName; set => shortName = value; }

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    private string longName;

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    public string LongName { get => longName; set => longName = value; }

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    private object defaultValue;

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    public object DefaultValue { get => defaultValue; set => defaultValue = value; }

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    private string helpText;

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    public string HelpText { get => helpText; set => helpText = value; }

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    private string validValues;

    [MirrorVerified("PtcThirdPartyArgumentAttributeMechanicalDifferentialTests")]
    public string ValidValues { get => validValues; set => validValues = value; }

    [MirrorVerified("Release IL")]
    public bool DefaultShortName => shortName == null;

    [MirrorVerified("Release IL")]
    public bool DefaultLongName => longName == null;

    [MirrorVerified("Release IL")]
    public bool HasDefaultValue => defaultValue != null;

    [MirrorVerified("Release IL")]
    public bool HasHelpText => helpText != null;
}
