#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class PreemptiveCommandBinding
{
    [MirrorVerified("PtcWpfPreemptiveCommandBindingMechanicalDifferentialTests")]
    private static void EmptyEventHandler(object sender, EventArgs e) { }

    [MirrorVerified("Release IL")]
    public void Add()
    {
        if (_bindingMarksEventAsHandled)
        {
            _element.CommandBindings.Add(_binding);
            return;
        }
        if (_canExecuteWrapper != null)
        {
            System.Windows.Input.CommandManager.AddPreviewCanExecuteHandler(_element, _canExecuteWrapper);
        }
        if (_executedWrapper != null)
        {
            System.Windows.Input.CommandManager.AddPreviewExecutedHandler(_element, _executedWrapper);
        }
    }

    [MirrorVerified("Release IL")]
    public void Remove()
    {
        if (_bindingMarksEventAsHandled)
        {
            _element.CommandBindings.Remove(_binding);
            return;
        }
        if (_canExecuteWrapper != null)
        {
            System.Windows.Input.CommandManager.RemovePreviewCanExecuteHandler(_element, _canExecuteWrapper);
        }
        if (_executedWrapper != null)
        {
            System.Windows.Input.CommandManager.RemovePreviewExecutedHandler(_element, _executedWrapper);
        }
    }
}
