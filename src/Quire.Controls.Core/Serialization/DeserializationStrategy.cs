#nullable disable
using System.Windows;
using System.Xml;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class DeserializationStrategy
{
    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    protected virtual bool IsRegionNotAffectingCalculation(UIElement element) => false;

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    public virtual void DeserializeRegionProlog(UIElement region) { }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    public virtual void DeserializeRegionsEpilog(XmlDocument xmlContentDocument) { }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    protected DeserializationStrategy() { }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    public IMathFormat DeserializeMathFormat { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { get; set; }
}
