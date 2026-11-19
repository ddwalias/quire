using System.Drawing;
using System.Windows;
using System.Windows.Shapes;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class Caret : FrameworkElement
{
    private Line _line = new Line();
    private bool _blink;

    public Line Line => _line;

    public bool Blink
    {
        get
        {
            if (_blink)
            {
                return IsCaretBlinkEnabled();
            }
            return false;
        }
        set
        {
            _blink = value;
            UpdateCaretBlinking();
        }
    }

    public Color Color
    {
        get
        {
            return (Color)GetValue(ColorProperty);
        }
        set
        {
            SetValue(ColorProperty, value);
            InitializeLine();
        }
    }

    private static void OnBlinkPropertyChanged(DependencyObject caret, DependencyPropertyChangedEventArgs e)
    {
        ((Caret)caret).UpdateCaretBlinking();
    }

    public Caret()
    {
        BindProperties();
        InitializeLine();
        Blink = true;
    }

    private void BindProperties()
    {
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, ColorProperty, "CaretColor");
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, BlinkPeriodProperty, "CaretBlinkPeriod");
    }

    private void InitializeLine()
    {
        _line.BeginInit();
        _line.Stroke = WpfUtils.CreateSolidWpfBrush((Color)GetValue(ColorProperty));
        _line.StrokeThickness = 1.0;
        _line.EndInit();
    }

    private void UpdateCaretBlinking()
    {
        int period = 0;
        if (Blink)
        {
            period = (int)GetValue(BlinkPeriodProperty);
        }
        WpfUtils.Blink(_line, period);
    }

    protected bool IsCaretBlinkEnabled()
    {
        return MathcadSettings.Instance.CaretBlink;
    }
}
