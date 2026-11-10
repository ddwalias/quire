namespace Quire.Controls.InputControls;

public partial class InputScriptableControlViewModel
{
    public InputScriptableControlViewModel(IInputScriptableControlModel model)
        : base(model)
    {
    }

    private IInputScriptableControlModel Model => (IInputScriptableControlModel)_model;
}
