using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Quire.Controls.CustomcolorPicker;

public partial class CustomColorCanvas
{
    private const double CursorKeyboardYoffset = 17d;
    private double _cursorKeyboardScaleX = 0.15;
    private double _cursorKeyboardScaleY = -0.15;
    private const int TargetScreenSizeHorizontal = 1920;
    private const int TargetScreenSizeVertical = 1050;
    private StreamResourceInfo cursorMouseStreamInfo;
    private Cursor cursorMouse;
    private const double MaxDegrees = 360d;
    private const double MaxPresent = 100d;
    internal global::Quire.Controls.CustomcolorPicker.CustomColorCanvas customColorCanvas;
    internal global::System.Windows.Controls.Canvas canvas;
    internal Path cursorKeyboard;

    public double Saturation
    {
        get
        {
            return (double)GetValue(SaturationProperty);
        }
        set
        {
            SetValue(SaturationProperty, value);
        }
    }

    public double Hue
    {
        get
        {
            return (double)GetValue(HueProperty);
        }
        set
        {
            SetValue(HueProperty, value);
        }
    }

    public UIElement Canvas => canvas;

    public event PropertyChangedEventHandler PropertyChanged;

    public void Dispose()
    {
        cursorMouse.Dispose();
        cursorMouseStreamInfo.Stream?.Dispose();
    }

    private void Canvas_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        Cursor = Cursors.Arrow;
        if (Keyboard.FocusedElement == sender)
        {
            cursorKeyboard.Visibility = Visibility.Visible;
        }
        else
        {
            Keyboard.Focus(Canvas);
        }
    }

    private void Canvas_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
        {
            HandleArrowKeys(e);
        }
    }

    private void Canvas_OnMouseEnter(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Cursor = cursorMouse;
            cursorKeyboard.Visibility = Visibility.Hidden;
        }
    }
}
