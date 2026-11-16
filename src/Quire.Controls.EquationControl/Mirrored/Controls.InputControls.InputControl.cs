using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.InputControls;

public partial class InputControl
{
    protected IInputControlViewModel _viewModel;

    public InputControl()
    {
    }

    public IInputControlViewModel ViewModel => _viewModel;

    public void EditingCompleted(bool shouldKeepFocus)
    {
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);
        e.Handled = true;
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs mouseEventArgs)
    {
        if (!ViewModel.IsFunctionalityEnabled)
        {
            FunctionalitiesLimitationCommands.DisplayFreemiumAboutBox.Execute(
                new DisplayFreemiumAboutBoxCommandParameter(this, EnumFunctionalityKey.InsertInputControlCommand),
                null);
            RaiseEvent(new CustomRoutedEventArgs<MouseButtonEventArgs>(CommonRoutedEvents.BlockedPreviewMouseDownEvent, mouseEventArgs));
            mouseEventArgs.Handled = true;
        }
        else
        {
            base.OnPreviewMouseLeftButtonDown(mouseEventArgs);
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (!ViewModel.IsFunctionalityEnabled && (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.PageDown || e.Key == Key.PageUp))
        {
            FunctionalitiesLimitationCommands.DisplayFreemiumAboutBox.Execute(
                new DisplayFreemiumAboutBoxCommandParameter(this, EnumFunctionalityKey.InsertInputControlCommand),
                null);
            e.Handled = true;
        }
        else
        {
            base.OnPreviewKeyDown(e);
        }
    }

    public void Resize(object sender, DragDeltaEventArgs e)
    {
        SetValue(WidthProperty, Math.Max(ActualWidth + e.HorizontalChange, MinWidth));
        SetValue(HeightProperty, Math.Max(ActualHeight + e.VerticalChange, MinHeight));
    }
}
