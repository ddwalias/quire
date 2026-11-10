using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Quire.Controls.InputControls;

public partial class InputControl
{
    public IInputControlViewModel ViewModel => _viewModel;

    public void EditingCompleted(bool shouldKeepFocus)
    {
    }

    public void Resize(object sender, DragDeltaEventArgs e)
    {
        SetValue(WidthProperty, Math.Max(ActualWidth + e.HorizontalChange, MinWidth));
        SetValue(HeightProperty, Math.Max(ActualHeight + e.VerticalChange, MinHeight));
    }
}
