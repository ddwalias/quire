using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Controls.CustomcolorPicker;

namespace Quire.Controls;

public partial class CustomColorPicker
{
    private const int SliderDrawingOffset = 5;
    private CustomColorPickerViewModel _dataContext;
    private readonly HashSet<ValidationError> _activeErrors = new HashSet<ValidationError>();
    internal global::System.Windows.Controls.DockPanel mainDockPanel;
    internal global::System.Windows.Controls.TextBox tbNew;
    internal global::System.Windows.Controls.TextBox tbCurrent;
    internal global::Fluent.Button ResetBtn;
    internal global::System.Windows.Controls.TextBox tbRed;
    internal global::System.Windows.Controls.Slider sliderRed;
    internal global::System.Windows.Controls.TextBox tbGreen;
    internal global::System.Windows.Controls.Slider sliderGreen;
    internal global::System.Windows.Controls.TextBox tbBlue;
    internal global::System.Windows.Controls.Slider sliderBlue;
    internal global::System.Windows.Controls.TextBox tbHue;
    internal global::System.Windows.Controls.Slider sliderHue;
    internal global::System.Windows.Controls.TextBox tbSat;
    internal global::System.Windows.Controls.Slider sliderSat;
    internal global::System.Windows.Controls.TextBox tbLum;
    internal global::System.Windows.Controls.Slider sliderLum;
    internal global::Quire.Controls.CustomcolorPicker.CustomColorCanvas colorMap;

    private void RgbTextBox_OnKeyDownText(object sender, KeyEventArgs e)
    {
        if (((TextBox)sender).Tag is Slider sender2 && ((int)e.Key == 26 || (int)e.Key == 24))
        {
            Slider_OnKeyDown(sender2, e);
        }
    }

    private void Slider_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is Slider slider)
        {
            double num = slider.Value;
            if ((int)e.Key == 26 || (int)e.Key == 25)
            {
                num++;
            }
            else if ((int)e.Key == 24 || (int)e.Key == 23)
            {
                num--;
            }
            if (num > slider.Maximum)
            {
                num = slider.Maximum;
            }
            else if (num < slider.Minimum)
            {
                num = slider.Minimum;
            }
            slider.Value = num;
        }
    }

    private void ResetBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _dataContext.Reset();
    }

    private void CustomColorPicker_OnLoaded(object sender, RoutedEventArgs e)
    {
        Keyboard.Focus(colorMap.Canvas);
    }

    private void TextBox_OnError(object sender, ValidationErrorEventArgs e)
    {
        switch (e.Action)
        {
            case ValidationErrorEventAction.Added:
                _activeErrors.Add(e.Error);
                break;
            case ValidationErrorEventAction.Removed:
                _activeErrors.Remove(e.Error);
                break;
        }

        _dataContext.OkIsEnabled = _activeErrors.Count == 0;
    }
}
