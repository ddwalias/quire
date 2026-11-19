using System.Windows;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;

namespace Quire.Behaviors;

public partial class UIFunctionalityWithCommandAttachedBehavior
{
    private ICommandSource AssociatedObjectAsCommandSource { get; set; }

    public IInputElement CommandTarget
    {
        get => (IInputElement)GetValue(CommandTargetProperty);
        set => SetValue(CommandTargetProperty, value);
    }

    private bool ShouldReplaceToFreemiumIcon(RoutedUICommand command)
    {
        if (command.Name == "ChangeAlgorithm")
        {
            return false;
        }

        return true;
    }

    private static void OnCommandTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = d as UIFunctionalityWithCommandAttachedBehavior;
        if (behavior != null && behavior.AssociatedObjectAsCommandSource != null)
        {
            var commandWrapper = behavior.AssociatedObjectAsCommandSource.Command as UIFunctionalityCommandWrapper;
            if (commandWrapper != null)
            {
                commandWrapper.CommandTarget = e.NewValue as IInputElement;
            }
        }
    }

    public UIFunctionalityWithCommandAttachedBehavior() { }
}
