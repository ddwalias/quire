#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IListResultNode<T>
{
    global::System.Collections.Generic.IList<T> List { get; }
    T NeutralValue { get; }
    T this[int index] { get; }
    bool IsMeaningfulValue(T valueIndex);
}
