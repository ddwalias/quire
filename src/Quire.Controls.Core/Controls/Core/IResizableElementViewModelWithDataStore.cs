#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IResizableElementViewModelWithDataStore
{
    IResizeData StoreResizeData(Size oldSize, Size newSize);
    void RestoreResizeData(IResizeData resizeData);
}
