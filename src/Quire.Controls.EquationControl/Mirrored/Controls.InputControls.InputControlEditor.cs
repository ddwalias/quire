using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Quire.Controls.InputControls;

public partial class InputControlEditor
{
    protected void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        _viewModel.CancelScript();
        DialogResult = false;
    }

    private void ButtonUndo_OnClick(object sender, RoutedEventArgs e)
    {
        tbScript.Undo();
    }

    private void ButtonRedo_OnClick(object sender, RoutedEventArgs e)
    {
        if (tbScript.CanRedo)
        {
            tbScript.Redo();
        }
    }

    private void TbScript_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.IsApplyEnabled = true;
    }

    private void TbScript_OnPreviewKeyDown(object sender, InputEventArgs e)
    {
        _viewModel.OnPreviewKeyDown(sender, e);
    }

    private void InfoMsgHide(object sender, RoutedEventArgs e)
    {
        _viewModel.IsInfoMsgHide = true;
    }

    protected void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.IsApplyEnabled)
        {
            _viewModel.InputScript = tbScript.Text;
            _viewModel.ApplyScript();
        }

        if (_viewModel.ValidationError == null && IsVisible)
        {
            DialogResult = true;
        }
    }

    private void OnApplyButtonClick(object sender, RoutedEventArgs e)
    {
        _viewModel.InputScript = tbScript.Text;
        _viewModel.ApplyScript();
        if (_viewModel.ValidationError == null)
        {
            _viewModel.IsApplyEnabled = false;
        }
    }

    private void ButtonFontSizeIncrease_OnClick(object sender, RoutedEventArgs e)
    {
        if (tbScript.FontSize < 25)
        {
            tbScript.FontSize += 1;
        }
    }

    private void ButtonFontSizeDecrease_OnClick(object sender, RoutedEventArgs e)
    {
        if (tbScript.FontSize > 5)
        {
            tbScript.FontSize -= 1;
        }
    }
}
