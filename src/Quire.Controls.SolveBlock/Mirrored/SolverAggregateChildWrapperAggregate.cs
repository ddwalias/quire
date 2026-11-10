namespace Quire.Controls;

internal partial class SolverAggregateChildWrapperAggregate
{
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableMoved;

    public global::Quire.Calculation.Core.ICalculatableAggregate WrappedCalculatableAggregate =>
        (global::Quire.Calculation.Core.ICalculatableAggregate)WrappedCalculatable;

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren =>
        WrappedCalculatableAggregate.CalculatableChildren;

    public SolverAggregateChildWrapperAggregate(global::Quire.Calculation.Core.ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
    }
}
