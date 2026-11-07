#nullable disable
using System;
using System.Windows;
using System.Windows.Input;

namespace Quire.Controls.Core;

public partial class ControlsWindow : Window
{
    public static readonly RoutedUICommand DisplayHelpCommand = new RoutedUICommand();

    public ControlsWindow()
    {
        CommandBindings.Add(new CommandBinding(DisplayHelpCommand, HelpCommandHandler));
    }

    private void HelpCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        DisplayHelpCommand.Execute(e.Parameter, Owner);
    }

    protected void DialogLoaded()
    {
        if (DebugTools.DevAssert(ActualWidth.IsGreaterThanOrEqual(MinWidth), null).Passed)
        {
            MinWidth = ActualWidth;
        }
        MaxWidth = Math.Max(Owner.ActualWidth, MinWidth);
        if (DebugTools.DevAssert(ActualHeight.IsGreaterThanOrEqual(MinHeight), null).Passed)
        {
            MaxHeight = MinHeight = ActualHeight;
        }
    }

    protected virtual void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
