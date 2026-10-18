#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class TextFormatSerializer
{
    [MirrorVerified("PtcControlsCoreSettingsTextFormatSerializerMechanicalDifferentialTests")]
    public TextDecorationsToAttributesSerializer TextDecorationSerializer { get; set; }

    [MirrorVerified("PtcControlsCoreSettingsTextFormatSerializerMechanicalDifferentialTests")]
    public string StyleKey { get; set; }
}
