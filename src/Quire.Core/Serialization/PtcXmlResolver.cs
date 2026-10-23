#nullable disable
using System.IO;
using System.Net;
using System.Reflection;
using System.Net;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Serialization;

public partial class PtcXmlResolver
{
    protected abstract string SchemaName(Uri absoluteUri);

    [MirrorVerified("Release IL")]
    public Stream GetStream(string schemaName)
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string name = string.Format(_fullSchemaNameFormat, schemaName);
        return executingAssembly.GetManifestResourceStream(name);
    }

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
