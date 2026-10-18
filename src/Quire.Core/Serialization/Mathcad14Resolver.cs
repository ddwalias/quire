#nullable disable
using System.IO;
using Quire.Mirror;

namespace Quire.Serialization;

public partial class Mathcad14Resolver
{
    [MirrorVerified("PtcSerializationMathcad14ResolverMechanicalDifferentialTests")]
    public Mathcad14Resolver() : base("Ptc.Mathcad14.schema.{0}") { }

    [MirrorVerified("PtcSerializationMathcad14ResolverMechanicalDifferentialTests")]
    public Stream MainStream => GetStream("worksheet30.xsd");
}
