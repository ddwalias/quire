#nullable disable
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
}
