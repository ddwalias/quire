namespace Quire.Controls.InputControls;

public partial class BaseScriptableInputControl
{
    public BaseScriptableInputControl()
    {
    }

    public IInputScriptableControlViewModel ViewModel => (IInputScriptableControlViewModel)_viewModel;
}
