using System.Drawing;
using System.Windows;
using System.Windows.Shapes;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class Caret : FrameworkElement
{
    public const int CARET_WIDTH = 1;

    public static DependencyProperty ColorProperty;

    public static DependencyProperty BlinkProperty;

    public static DependencyProperty BlinkPeriodProperty;

    static Caret()
    {
        ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(Caret));
        BlinkProperty = DependencyProperty.Register("Blink", typeof(bool), typeof(Caret), new FrameworkPropertyMetadata
        {
            DefaultValue = false,
            PropertyChangedCallback = OnBlinkPropertyChanged,
        });
        BlinkPeriodProperty = DependencyProperty.Register("BlinkPeriod", typeof(int), typeof(Caret));
    }

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
