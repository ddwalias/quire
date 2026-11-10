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
}
