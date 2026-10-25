#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public abstract partial class DeserializationStrategyWithMathFormat : DeserializationStrategy
{
    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyWithMathFormatMechanicalDifferentialTests")]
    protected DeserializationStrategyWithMathFormat() { }

    public override void DeserializeRegionProlog(UIElement region)
    {
        IMathFormatForDeserialization mathFormatForDeserialization = region as IMathFormatForDeserialization;
        if (mathFormatForDeserialization == null)
        {
            return;
        }
        if (DeserializeMathFormat != null)
        {
            mathFormatForDeserialization.DeserializeMathFormat = DeserializeMathFormat;
        }
        if (DeserializeLabeledIdFormat != null)
        {
            mathFormatForDeserialization.DeserializeLabeledIdFormat = DeserializeLabeledIdFormat;
        }
    }
}
