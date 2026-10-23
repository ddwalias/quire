#nullable disable
using System.IO;
using System;
using System.Xml;
using Quire.Mirror;

namespace Quire.Serialization;

public partial class Mathcad14Resolver
{
    [MirrorVerified("PtcSerializationMathcad14ResolverMechanicalDifferentialTests")]
    public Mathcad14Resolver() : base("Ptc.Mathcad14.schema.{0}") { }

    [MirrorVerified("PtcSerializationMathcad14ResolverMechanicalDifferentialTests")]
    public Stream MainStream => GetStream("worksheet30.xsd");

    public static void InitializeDocument(XmlDocument xmlMetadata)
    {
        Mathcad14Resolver mathcad14Resolver = new Mathcad14Resolver();
        XmlReaderSettings settings = new XmlReaderSettings
        {
            ProhibitDtd = false
        };
        using XmlReader schemaDocument = XmlReader.Create(mathcad14Resolver.MainStream, settings);
        xmlMetadata.Schemas.XmlResolver = mathcad14Resolver;
        xmlMetadata.Schemas.Add(null, schemaDocument);
        xmlMetadata.Schemas.Compile();
    }

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
