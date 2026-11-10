namespace Quire.Controls;

internal partial class SolverAggregateChildWrapperAggregate
{
    public global::Quire.Calculation.Core.ICalculatableAggregate WrappedCalculatableAggregate =>
        (global::Quire.Calculation.Core.ICalculatableAggregate)WrappedCalculatable;

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren =>
        WrappedCalculatableAggregate.CalculatableChildren;

    public SolverAggregateChildWrapperAggregate(global::Quire.Calculation.Core.ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
    }
}
