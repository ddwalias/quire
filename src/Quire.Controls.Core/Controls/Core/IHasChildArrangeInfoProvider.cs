#nullable disable
namespace Quire.Controls.Core;

public partial interface IHasChildArrangeInfoProvider
{
    IUIElementArrangeInfoProvider ChildArrangeInfoProvider { get; }
}
