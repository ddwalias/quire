namespace Quire.Controls;

internal partial class SolveBlockCalculatable
{
    public static int FindMatchingParenthesis(int startParenPos, string str)
    {
        int nesting = 1;
        for (int i = startParenPos + 1; i < str.Length; i++)
        {
            if (str[i] == ')')
                nesting--;
            else if (str[i] == '(')
                nesting++;

            if (nesting == 0)
                return i;
        }

        return -1;
    }

    public static SolveBlockEquationTypes GetEquationTypeOfCalc(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (calc == null)
            return (SolveBlockEquationTypes)0;

        if (calc is not global::Quire.Calculation.Core.ICalculatableAggregate aggregate ||
            global::Quire.EnumerableExtensions.IsNullOrEmpty(aggregate.CalculatableChildren))
            return GetEquationTypeOfRl(calc.Rl);

        return GetEquationTypeOfRl(
            SolveBlockCalculatableAggregate.ComposeSolveblockChildRlFromEquationControl(
                global::Quire.Calculation.Core.CalculatableAggregateExtensions.GetAggregateRl(aggregate)));
    }
}
