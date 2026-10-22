#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalculatableContainer
{
    global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren { get; }
    event global::System.Action<global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;
    event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;
    event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableMoved;
}
