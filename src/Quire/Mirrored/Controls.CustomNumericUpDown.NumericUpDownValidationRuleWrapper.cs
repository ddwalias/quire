using System.Windows;

namespace Quire.Controls.CustomNumericUpDown;

public partial class NumericUpDownValidationRuleWrapper : DependencyObject
{
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
        "MinValue",
        typeof(int),
        typeof(NumericUpDownValidationRuleWrapper),
        new FrameworkPropertyMetadata((object)int.MinValue));

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
        "MaxValue",
        typeof(int),
        typeof(NumericUpDownValidationRuleWrapper),
        new FrameworkPropertyMetadata((object)int.MaxValue));

    public int MinValue
    {
        get => (int)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    public int MaxValue
    {
        get => (int)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public NumericUpDownValidationRuleWrapper()
    {
    }
}
