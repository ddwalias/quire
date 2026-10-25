#nullable disable
using System.Windows;
using System.Windows.Controls;

namespace Quire.Controls.Core;

public partial class PrimeContextMenuItem : MenuItem
{
    public static readonly DependencyProperty PrimeContextMenuItemStyleProperty = DependencyProperty.Register("PrimeContextMenuItemStyle", typeof(Style), typeof(PrimeContextMenuItem), new PropertyMetadata(new PropertyChangedCallback(HandlePrimeContextMenuItemStyleChanged)));

    public Style PrimeContextMenuItemStyle
    {
        get
        {
            return GetValue(PrimeContextMenuItemStyleProperty) as Style;
        }
        set
        {
            SetValue(PrimeContextMenuItemStyleProperty, value);
        }
    }

    private static void HandlePrimeContextMenuItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != null)
        {
            d.SetValue(FrameworkElement.StyleProperty, e.NewValue);
        }
    }

    public PrimeContextMenuItem()
    {
    }
}
