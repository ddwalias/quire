#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class ConversionAnnotationMessage
{
    [MirrorVerified("PtcControlsCoreSerializationConversionAnnotationMessageMechanicalDifferentialTests")]
    public override string ToString() => Text;

    [MirrorVerified("PtcControlsCoreSerializationConversionAnnotationMessageMechanicalDifferentialTests")]
    public ConversionAnnotationMessage() { }

    [MirrorVerified("PtcControlsCoreSerializationConversionAnnotationMessageMechanicalDifferentialTests")]
    public string CategoryKey { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationConversionAnnotationMessageMechanicalDifferentialTests")]
    public string Text { get; set; }
}
