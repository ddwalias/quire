#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public sealed partial class TextDecorationsToAttributesSerializer
{
    [MirrorVerified("PtcControlsCoreSettingsTextDecorationsToAttributesSerializerMechanicalDifferentialTests")]
    public TextDecorationCollection TextDecorationsCollection { get; set; }
}
