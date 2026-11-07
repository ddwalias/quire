#nullable disable
namespace Quire.Controls.Core;

public partial interface IResizableElementInfo
{
    bool PreserveAspectRatio { get; }
    SubElementToResizeData SubElementToResizeData { get; }
}
