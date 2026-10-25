#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Quire.Controls.Core;

public abstract partial class VisibilityAwareAdorner : Adorner
{
    protected VisibilityAwareAdorner(UIElement adornedElement, bool trackVisibility)
        : base(adornedElement)
    {
        if (trackVisibility)
        {
            SetBinding(UIElement.VisibilityProperty, new Binding
            {
                Path = new PropertyPath(UIElement.IsVisibleProperty),
                Source = AdornedElement,
                Converter = new BooleanToVisibilityConverter()
            });
        }
    }
}
