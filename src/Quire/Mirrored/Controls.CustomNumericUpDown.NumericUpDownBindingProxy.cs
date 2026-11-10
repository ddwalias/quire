using System.Windows;

namespace Quire.Controls.CustomNumericUpDown;

public partial class NumericUpDownBindingProxy : Freezable
{
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(NumericUpDownBindingProxy), new PropertyMetadata((PropertyChangedCallback)null));

    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new NumericUpDownBindingProxy();
    }

    public NumericUpDownBindingProxy()
    {
    }
}
