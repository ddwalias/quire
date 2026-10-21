using System.Windows.Markup;
using System.IO.Packaging;

namespace Quire.Core;

public static partial class ParserContextFactory
{
    public static ParserContext Create() => new ParserContext { XamlTypeMapper = _xamlMapper };

    public static ParserContext Create(Uri packageUri, Uri partUri)
    {
        ParserContext parserContext = Create();
        parserContext.BaseUri = PackUriHelper.Create(packageUri, partUri);
        return parserContext;
    }
}
