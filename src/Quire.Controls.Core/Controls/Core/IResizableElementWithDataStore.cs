#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IResizableElementWithDataStore
{
    IResizeData StoreResizeData(Size oldSize, Size newSize);
    void RestoreResizeData(IResizeData resizeData);
    Size CoerceNewSize(Size newSize, IResizeData resizeData);
}
