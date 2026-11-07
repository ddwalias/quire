#nullable disable
namespace Quire.Controls.Core.Whiteboard;

public partial interface IWhiteboardItemContainerWithActiveItem
{
    IWhiteboardItem ActiveItem { get; set; }
}
