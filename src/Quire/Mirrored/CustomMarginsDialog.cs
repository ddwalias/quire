using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core;

namespace Quire;

public partial class CustomMarginsDialog
{
    private readonly CustomMarginsViewModel _viewModel;
    private SpiritMainWindow _ownerWindow;
    internal TextBox LeftMarginTextBox;
    internal TextBox RightMarginTextBox;
    internal TextBox TopMarginTextBox;
    internal TextBox BottomMarginTextBox;

    public Thickness NewPageMargin => _viewModel.NewPageMargin;

    public static void ShowDialog(SpiritMainWindow owner, FrameworkElement positionProvider, WorksheetPageLayout pageLayout)
    {
        CustomMarginsDialog dialog = new CustomMarginsDialog(owner, pageLayout, new FrameworkElementPositionWrapper(positionProvider));
        if (dialog.ShowDialog() == true)
        {
            pageLayout.SetPageMarginInPageSizeUnits(dialog.NewPageMargin);
            if (pageLayout.MarginType == PageMarginType.Custom)
            {
                dialog._ownerWindow.LastCustomMarginValueSetting = pageLayout.PageMargin;
            }
        }
    }

    private void MarginTextBox_TextChanged(object sender, TextChangedEventArgs args)
    {
        _viewModel.ValidateInput();
    }

    private void OkButton_Click(object sender, RoutedEventArgs args)
    {
        if (_viewModel.HasErrors)
        {
            _viewModel.FixPageMargins();
        }
        else
        {
            DialogResult = true;
        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        // Release IL tests SizeToContent with bne.un and skips to the return, so the guard is nested.
        if (SizeToContent == SizeToContent.WidthAndHeight)
        {
            if (sizeInfo.NewSize.Width.IsGreaterThanOrEqual(MinWidth) && sizeInfo.NewSize.Height.IsGreaterThanOrEqual(MinHeight))
            {
                SizeToContent = SizeToContent.Manual;
                DebugTools.DevAssert(sizeInfo.PreviousSize == default, null);
            }
        }
    }
}
