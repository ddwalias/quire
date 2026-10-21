using System.Windows.Markup;

namespace Quire.Core;

public static partial class ParserContextFactory
{
    public static ParserContext Create() => new ParserContext { XamlTypeMapper = _xamlMapper };
}
