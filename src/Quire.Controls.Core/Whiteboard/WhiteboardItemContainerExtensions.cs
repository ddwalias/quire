#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core.Serialization;
using Quire.Services;

namespace Quire.Controls.Core.Whiteboard;

public static partial class WhiteboardItemContainerExtensions
{
    private static IContainerItemManager _itemManager;

    public static IContainerItemManager ItemManager
    {
        get
        {
            if (_itemManager == null || !_itemManager.IsConnected)
            {
                return _itemManager = AppServiceRegistry.GetService<IContainerItemManager>();
            }
            return _itemManager;
        }
    }

    public static bool HasRegionsWithConverterAnnotations(this IWhiteboardItemContainer container)
    {
        if (!container.WhiteboardItems.OfType<UIElement>().Any(MC14ConversionAnnotation.HasConversionAnnotations))
        {
            return container.WhiteboardItems.OfType<IHasRegionsWithConverterAnnotations>().Any((IHasRegionsWithConverterAnnotations item) => item.HasRegionsWithConverterAnnotations);
        }
        return true;
    }

    public static IEnumerable<UIElement> RegionsWithConverterAnnotations(this IWhiteboardItemContainer container)
    {
        return container.WhiteboardItems.OfType<UIElement>().Where(MC14ConversionAnnotation.HasConversionAnnotations).Concat(container.WhiteboardItems.OfType<IHasRegionsWithConverterAnnotations>().SelectMany((IHasRegionsWithConverterAnnotations item) => item.RegionsWithConverterAnnotations));
    }
}
