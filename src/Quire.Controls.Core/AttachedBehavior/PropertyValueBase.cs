#nullable disable
using System;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertyValueBase
{
    private static readonly Type _typeOfThis = typeof(PropertyValueBase);

    public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof(object), _typeOfThis, new PropertyMetadata((PropertyChangedCallback)null));

    public PropertyValueBase() { }

    public object PropertyValue
    {
        get => GetValue(PropertyValueProperty);
        set => SetValue(PropertyValueProperty, value);
    }
}
