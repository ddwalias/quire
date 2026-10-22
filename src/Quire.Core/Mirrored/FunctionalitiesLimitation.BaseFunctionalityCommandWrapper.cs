#nullable disable
#pragma warning disable

namespace Quire.FunctionalitiesLimitation;

partial class BaseFunctionalityCommandWrapper
{
    public virtual event global::System.EventHandler CanExecuteChanged
    {
        add => NestedCommand.CanExecuteChanged += value;
        remove => NestedCommand.CanExecuteChanged -= value;
    }
    protected BaseFunctionalityCommandWrapper(global::System.Windows.Input.RoutedUICommand nestedCommand)
    {
        NestedCommand = nestedCommand;
        if (nestedCommand is global::Quire.Wpf.RoutedUICommandWithFunctionalityKey)
        {
            FunctionalityKey = ((global::Quire.Wpf.RoutedUICommandWithFunctionalityKey)nestedCommand).FunctionalityKey;
        }
        else
        {
            global::Quire.DebugTools.FailAssertion(global::System.String.Format("{0} Nested command is not of RoutedUICommandWithFunctionalityKey type, perhaps it wasn't changed yet.", nestedCommand), global::System.Array.Empty<object>());
        }
    }
}
