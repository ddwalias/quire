namespace Quire.Calculation;

/// <summary>Converts engine results (values in SI) into display-ready result nodes for a unit system. The conversion
/// is the mirrored <see cref="IValueToIResultNodeTransform"/> part; the product display records it used to build are
/// gone (D21).</summary>
public partial class ValueToIResultNodeTransform
{
    /// <summary>Declared explicitly: the mirrored part adds Mathcad's constructors, which would hide the implicit one.</summary>
    public ValueToIResultNodeTransform() { }
}
