namespace Quire.Controls;

internal partial class SolveBlockCalculatableAggregate
{
    private static string GetSymbolicEvalExpression(global::System.Text.RegularExpressions.Match symbolicEvalMatch)
    {
        if (symbolicEvalMatch == null || !symbolicEvalMatch.Success)
            return null;

        string expression = symbolicEvalMatch.Groups["expression"].Value;
        if (expression.StartsWith("(eval_range_loops"))
        {
            expression = expression.Substring("(eval_range_loops".Length);
            expression = expression.Remove(expression.Length - 1);
        }

        return expression;
    }

    public global::Quire.Calculation.Core.ICalculatableAggregate WrappedCalculatableAggregate =>
        (global::Quire.Calculation.Core.ICalculatableAggregate)WrappedCalculatable;

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren =>
        WrappedCalculatableAggregate.CalculatableChildren;

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
