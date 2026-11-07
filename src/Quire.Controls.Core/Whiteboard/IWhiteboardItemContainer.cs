#nullable disable
using System.Windows.Input;

namespace Quire.Controls.Core.Whiteboard;

public partial interface IWhiteboardItemContainer
{
    WhiteboardItemsCollection WhiteboardItems { get; }
    IWhiteboardItem ActiveItem { get; set; }
    bool CanAddItems { get; }
    bool IsTabNavigatable { get; }
    IContainerItemManager ContainerItemManager { get; }
    void AddItem(IWhiteboardItem item);
    void RemoveItem(IWhiteboardItem item);
    void TabInto(FocusNavigationDirection dir);
}
