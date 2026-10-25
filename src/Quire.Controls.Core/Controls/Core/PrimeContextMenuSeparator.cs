#nullable disable
using System.Windows;
using System.Windows.Controls;

namespace Quire.Controls.Core;

public partial class PrimeContextMenuSeparator : Separator
{
    public static readonly DependencyProperty PrimeContextMenuSeparatorStyleProperty = DependencyProperty.Register("PrimeContextMenuSeparatorStyle", typeof(Style), typeof(PrimeContextMenuSeparator), new PropertyMetadata(new PropertyChangedCallback(HandlePrimeContextMenuSeparatorStyleChanged)));

    public Style PrimeContextMenuSeparatorStyle
    {
        get
        {
            return GetValue(PrimeContextMenuSeparatorStyleProperty) as Style;
        }
        set
        {
            SetValue(PrimeContextMenuSeparatorStyleProperty, value);
        }
    }

    private static void HandlePrimeContextMenuSeparatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != null)
        {
            d.SetValue(FrameworkElement.StyleProperty, e.NewValue);
        }
    }

    public PrimeContextMenuSeparator()
    {
    }
}
