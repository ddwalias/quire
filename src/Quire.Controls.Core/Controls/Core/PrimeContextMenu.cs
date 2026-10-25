#nullable disable
using System.Windows;
using System.Windows.Controls;

namespace Quire.Controls.Core;

public partial class PrimeContextMenu : ContextMenu, IPrimeContextMenu
{
    public static readonly DependencyProperty PrimeContextMenuStyleProperty = DependencyProperty.Register("PrimeContextMenuStyle", typeof(Style), typeof(PrimeContextMenu), new PropertyMetadata(new PropertyChangedCallback(HandlePrimeContextMenuStyleChanged)));

    public Style PrimeContextMenuStyle
    {
        get
        {
            return GetValue(PrimeContextMenuStyleProperty) as Style;
        }
        set
        {
            SetValue(PrimeContextMenuStyleProperty, value);
        }
    }

    private static void HandlePrimeContextMenuStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != null)
        {
            d.SetValue(FrameworkElement.StyleProperty, e.NewValue);
        }
    }

    public PrimeContextMenu()
    {
    }
}
