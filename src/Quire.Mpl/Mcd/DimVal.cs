#nullable disable
using Quire.Mpl.Run;

namespace Quire.Mcd;

/// <summary>Base of every value the engine computes with; <see cref="m_dim"/> is its physical dimension.</summary>
public abstract partial class DimVal
{
    /// <summary>The dimension; null when the value is polymorphic (has no dimension of its own).</summary>
    public Dim m_dim;

    protected DimVal() { }

    public bool IsPolymorphic => m_dim == null;

    public virtual Dim PhysicalDimension => m_dim;

    public virtual bool IsDimless => Dim.Id_b(PhysicalDimension);

    /// <summary>The text when this is a string, otherwise null.</summary>
    public virtual string IsString => null;

    /// <summary>This value when it is a matrix, otherwise null.</summary>
    public virtual MatVal IsMatrix => null;

    /// <summary>This value when it is a function, otherwise null.</summary>
    public virtual Function IsFunction => null;

    /// <summary>Nonzero when this is a real number.</summary>
    public virtual int IsReal => 0;

    /// <summary>The number of values a range iterates over; 0 for anything else.</summary>
    public virtual int Rsize => 0;

    public virtual DimVal SetDim(Dim d) =>
        throw new InvalidOperationException($"{GetType().Name} has no physical dimension to set");

    public static Dim DimOf(object o) => o is DimVal v ? v.PhysicalDimension : Dim.identity;

    /// <summary>The value at position <paramref name="n"/> of a range.</summary>
    public virtual DimVal range_get(int n) => throw new InvalidOperationException($"{GetType().Name} is not a range");

    /// <summary>This value as a column vector; <paramref name="m"/> names the operation for errors and
    /// <paramref name="a"/> is the source position.</summary>
    public virtual MatVal asVector(string m, uint a) =>
        throw new InvalidCastException($"{m ?? "This operation"} needs a vector, not {GetType().Name}");

    public virtual string ToString(IFormatProvider culture) => ToString();
}
