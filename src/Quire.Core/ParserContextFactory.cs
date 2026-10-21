using System.Reflection;
using System.Windows.Markup;
using System.IO.Packaging;
using Quire.Mirror;

namespace Quire.Core;

public static partial class ParserContextFactory
{
    [MirrorVerified("Release IL")]
    private static readonly XamlTypeMapper _xamlMapper;

    [MirrorVerified("Release IL")]
    static ParserContextFactory()
    {
        _xamlMapper = new XamlTypeMapper(Array.Empty<string>());
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            Assembly assembly = assemblies[i];
            try
            {
                if (assembly.IsDynamic || assembly.FullName.StartsWith("System") || assembly.FullName.StartsWith("Microsoft"))
                {
                    continue;
                }
                string name = assembly.GetName().Name;
                _xamlMapper.AddMappingProcessingInstruction("clr-namespace:;assembly=" + name, "", name);
            }
            catch
            {
            }
        }
    }

    public static ParserContext Create() => new ParserContext { XamlTypeMapper = _xamlMapper };

    public static ParserContext Create(Uri packageUri, Uri partUri)
    {
        ParserContext parserContext = Create();
        parserContext.BaseUri = PackUriHelper.Create(packageUri, partUri);
        return parserContext;
    }
}
