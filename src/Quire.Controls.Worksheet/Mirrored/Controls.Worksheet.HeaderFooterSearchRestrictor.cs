using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class HeaderFooterSearchRestrictor
{
    private ISearchableRegion _searchableRegion;

    public bool CanSearch => _searchableRegion != null;

    public ISearchableRegion SearchableRegion => _searchableRegion;

    public static HeaderFooterSearchRestrictor CreateFindRestrictor(IHeaderFooter headerFooter)
    {
        return new HeaderFooterSearchRestrictor(headerFooter);
    }

    private HeaderFooterSearchRestrictor(IHeaderFooter headerFooter)
    {
        _searchableRegion = headerFooter as ISearchableRegion;
    }
}
