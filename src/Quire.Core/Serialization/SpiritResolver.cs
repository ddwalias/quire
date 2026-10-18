#nullable disable
using Quire.Mirror;

namespace Quire.Serialization;

public partial class SpiritResolver
{
    [MirrorVerified("PtcSerializationSpiritResolverMechanicalDifferentialTests")]
    private SpiritResolver() : base("Ptc.Schemas.{0}") { }
}
