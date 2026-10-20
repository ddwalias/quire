#nullable disable
using System.Net;
using System.Net;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Serialization;

public partial class PtcXmlResolver
{
    [MirrorVerified("PtcSerializationPtcXmlResolverMechanicalDifferentialTests")]
    protected string _fullSchemaNameFormat;

    [MirrorVerified("PtcSerializationPtcXmlResolverMechanicalDifferentialTests")]
    protected PtcXmlResolver(string fullSchemaNameFormat)
    {
        _fullSchemaNameFormat = fullSchemaNameFormat;
    }

    [MirrorVerified("PtcSerializationPtcXmlResolverMechanicalDifferentialTests")]
    public override ICredentials Credentials { set => throw new NotImplementedException(); }

    [MirrorVerified("Release IL")]
    public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn) => GetStream(SchemaName(absoluteUri));
}
