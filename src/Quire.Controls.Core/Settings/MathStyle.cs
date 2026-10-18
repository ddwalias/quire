#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class MathStyle
{
    [MirrorVerified("PtcControlsCoreSettingsMathStyleMechanicalDifferentialTests")]
    public IdLabels Label { get; set; }

    [MirrorVerified("PtcControlsCoreSettingsMathStyleMechanicalDifferentialTests")]
    public TextDecorationsToAttributesSerializer TextDecorationSerializer { get; set; }
}
