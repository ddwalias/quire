#nullable disable
using Quire.Services;

namespace Quire.Controls.Core.Whiteboard;

public partial class WhiteboardItemSearchRestrictor : IFindRestrictor
{
    private readonly IWhiteboardItem _whiteboardItem;
    private readonly ISearchableRegion _searchableRegion;
    private readonly IContainerItemManager _itemManager;

    public static WhiteboardItemSearchRestrictor TryCreateFindRestrictor(IWhiteboardItem whiteboardItem)
    {
        ISearchableRegion searchableRegion = whiteboardItem as ISearchableRegion;
        if (searchableRegion == null)
        {
            return null;
        }
        return new WhiteboardItemSearchRestrictor(searchableRegion, whiteboardItem);
    }

    public static WhiteboardItemSearchRestrictor CreateFindRestrictor(ISearchableRegion searchableRegion, IWhiteboardItem whiteboardItem)
    {
        return new WhiteboardItemSearchRestrictor(searchableRegion, whiteboardItem);
    }

    private WhiteboardItemSearchRestrictor(ISearchableRegion searchableRegion, IWhiteboardItem whiteboardItem)
    {
        _whiteboardItem = whiteboardItem;
        _searchableRegion = searchableRegion;
        _itemManager = AppServiceRegistry.GetService<IContainerItemManager>();
    }

    public bool CanSearch
    {
        get
        {
            if (_searchableRegion != null)
            {
                return _itemManager.GetCanActivate(_whiteboardItem);
            }
            return false;
        }
    }

    public ISearchableRegion SearchableRegion => _searchableRegion;
}
