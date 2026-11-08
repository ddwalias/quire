namespace Quire.Controls;

internal partial class SolveBlockCalculatableAggregate
{
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
