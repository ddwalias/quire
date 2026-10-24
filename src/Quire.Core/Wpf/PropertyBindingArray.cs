#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class PropertyBindingArray
{
    [MirrorVerified("PtcWpfPropertyBindingArrayMechanicalDifferentialTests")]
    private PropertyBinding[] _propertyBindings;

    [MirrorVerified("PtcWpfPropertyBindingArrayMechanicalDifferentialTests")]
    public PropertyBindingArray(params PropertyBinding[] propertyBindings)
    {
        _propertyBindings = propertyBindings;
    }

    public void Set(DependencyObject dependencyObject)
    {
        PropertyBinding[] propertyBindings = _propertyBindings;
        for (int i = 0; i < propertyBindings.Length; i++)
        {
            propertyBindings[i].Set(dependencyObject);
        }
    }

    public void Clear(DependencyObject dependencyObject)
    {
        PropertyBinding[] propertyBindings = _propertyBindings;
        for (int i = 0; i < propertyBindings.Length; i++)
        {
            propertyBindings[i].Clear(dependencyObject);
        }
    }
}
