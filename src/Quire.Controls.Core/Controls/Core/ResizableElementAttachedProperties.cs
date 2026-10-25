#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core;

public static partial class ResizableElementAttachedProperties
{
    private static readonly Type _typeOfThis = typeof(ResizableElementAttachedProperties);

    public static readonly DependencyProperty IsResizingProperty = DependencyProperty.RegisterAttached("IsResizing", typeof(bool), _typeOfThis);

    public static bool GetIsResizing(DependencyObject element)
    {
        return (bool)element.GetValue(IsResizingProperty);
    }

    public static void SetIsResizing(DependencyObject element, bool value)
    {
        if (value)
        {
            element.SetValue(IsResizingProperty, (object)true);
        }
        else
        {
            element.ClearValue(IsResizingProperty);
        }
    }
}
