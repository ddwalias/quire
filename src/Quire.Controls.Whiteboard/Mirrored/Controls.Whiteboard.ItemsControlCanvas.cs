#nullable disable
#pragma warning disable

using System.Windows;

namespace Quire.Controls.Whiteboard;

public partial class ItemsControlCanvas
{
    public ItemsControlCanvas() { }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        WhiteboardViewModel viewModel = DataContext as WhiteboardViewModel;
        if (viewModel != null)
        {
            return viewModel.ArrangeOverride(arrangeSize);
        }

        return base.ArrangeOverride(arrangeSize);
    }
}
