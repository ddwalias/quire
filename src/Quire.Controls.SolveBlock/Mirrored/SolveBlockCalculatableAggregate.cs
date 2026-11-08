namespace Quire.Controls;

internal partial class SolveBlockCalculatableAggregate
{
    private void HookupAggregateContainerEvents(global::Quire.Calculation.Core.ICalculatableAggregate aggregate)
    {
        aggregate.CalculatableAdded += HandleCalculatableAdded;
        aggregate.CalculatableRemoved += HandleCalculatableRemoved;
        aggregate.CalculatableMoved += HandleCalculatableMoved;
    }

    private void UnhookAggregateContainerEvents(global::Quire.Calculation.Core.ICalculatableAggregate aggregate)
    {
        aggregate.CalculatableAdded -= HandleCalculatableAdded;
        aggregate.CalculatableRemoved -= HandleCalculatableRemoved;
        aggregate.CalculatableMoved -= HandleCalculatableMoved;
    }

    public override void Dispose()
    {
        base.Dispose();
        UnhookAggregateContainerEvents(WrappedCalculatableAggregate);
    }

    protected override bool UpdateEquationType(global::Quire.Calculation.Core.ICalculatable calc)
    {
        return UpdateEquationTypeOfAggregateCalculatable(
            (global::Quire.Calculation.Core.ICalculatableAggregate)calc);
    }

    protected bool UpdateEquationTypeOfAggregateCalculatable(
        global::Quire.Calculation.Core.ICalculatableAggregate calc)
    {
        SolveBlockEquationTypes oldEquationType = EquationType;
        EquationType = SolveBlockCalculatable.GetEquationTypeOfCalc(calc);
        return EquationType != oldEquationType;
    }
}
