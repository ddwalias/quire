#nullable disable
using System.Windows;
using System.Windows.Controls;

namespace Quire.Controls.Core;

public partial class NumericUpDownWatermark : NumericUpDown
{
    public static readonly DependencyProperty WatermarkValueProperty = DependencyProperty.Register("WatermarkValue", typeof(string), typeof(NumericUpDownWatermark), new PropertyMetadata(""));

    public override int NumberValue
    {
        get => base.NumberValue;
        set
        {
            base.NumberValue = value;
            if (string.IsNullOrEmpty(ValueTextBox.Text))
            {
                NumericUpDown.OnNumberChanged(this, new DependencyPropertyChangedEventArgs(NumericUpDown.NumberValueProperty, value, value));
            }
        }
    }

    public string WatermarkValue
    {
        get => (string)GetValue(WatermarkValueProperty);
        set => SetValue(WatermarkValueProperty, value);
    }

    public NumericUpDownWatermark()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ValueTextBox != null)
        {
            ValueTextBox.TextChanged += WatermarkVisibilityToggle;
        }
    }

    private void WatermarkVisibilityToggle(object sender, TextChangedEventArgs textChangedEventArgs)
    {
        if (ValueTextBox != null)
        {
            WaterMarkTextBlock.Visibility = (IsTextBoxEmpty() ? Visibility.Visible : Visibility.Hidden);
        }
    }
}
