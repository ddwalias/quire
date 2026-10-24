#nullable disable
using System.Windows;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class PreemptiveCommandBinding
{
    [MirrorVerified("PtcWpfPreemptiveCommandBindingMechanicalDifferentialTests")]
    private static void EmptyEventHandler(object sender, EventArgs e) { }

    internal PreemptiveCommandBinding(UIElement element, ICommand command, ExecutedRoutedEventHandler previewExecuted, CanExecuteRoutedEventHandler previewCanExecute, bool bindingMarksEventAsHandled)
    {
        _element = element;
        _bindingMarksEventAsHandled = bindingMarksEventAsHandled;
        if (_bindingMarksEventAsHandled)
        {
            _binding = new CommandBinding(command, EmptyEventHandler);
            if (previewCanExecute != null)
            {
                _binding.CanExecute += previewCanExecute;
                _binding.PreviewCanExecute += previewCanExecute;
            }
            if (previewExecuted != null)
            {
                _binding.PreviewExecuted += previewExecuted;
                return;
            }
            // Keep the no-handler path at the constructor's shared return to match Release IL.
            goto ConstructorExit;
        }
        if (previewCanExecute != null)
        {
            _canExecuteWrapper = (object sender, CanExecuteRoutedEventArgs args) =>
            {
                if (args.Command == command)
                {
                    previewCanExecute(sender, args);
                }
            };
        }
        if (previewExecuted != null)
        {
            _executedWrapper = (object sender, ExecutedRoutedEventArgs args) =>
            {
                if (args.Command == command)
                {
                    previewExecuted(sender, args);
                }
            };
        }

    ConstructorExit:
        return;
    }

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
