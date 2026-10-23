#nullable disable
using System.IO;
using System;
using Quire.Mirror;

namespace Quire.Serialization;

public partial class Mathcad14Resolver
{
    [MirrorVerified("PtcSerializationMathcad14ResolverMechanicalDifferentialTests")]
    public Mathcad14Resolver() : base("Ptc.Mathcad14.schema.{0}") { }

    [MirrorVerified("PtcSerializationMathcad14ResolverMechanicalDifferentialTests")]
    public Stream MainStream => GetStream("worksheet30.xsd");

    protected override string SchemaName(Uri absoluteUri)
    {
        string text = absoluteUri.Segments[^1];
        if ("EN" == text)
        {
            text = "XMLSchema.dtd";
        }
        return text;
    }
}
