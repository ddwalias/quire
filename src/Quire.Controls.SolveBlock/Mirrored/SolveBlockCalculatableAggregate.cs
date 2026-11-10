namespace Quire.Controls;

internal partial class SolveBlockCalculatableAggregate
{
    private static readonly int[] SymbolicEvalIndices = new int[2] { 1, 2 };
    private const int DefineSymEvalAggregateCount = 3;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableMoved;

    public SolveBlockCalculatableAggregate(
        global::Quire.Calculation.Core.ICalculatableAggregate wrappedCalculatableAggregate)
        : base(wrappedCalculatableAggregate)
    {
        UpdateEquationTypeOfAggregateCalculatable(wrappedCalculatableAggregate);
        HookupAggregateContainerEvents(wrappedCalculatableAggregate);
    }

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

    internal static bool IsRlForSymbolicEval(
        global::System.Collections.Generic.IList<string> aggregateRl,
        out int symbolicEvalIndex)
    {
        symbolicEvalIndex = SymbolicEvalIndices[0];
        if (aggregateRl == null)
            return false;

        var regex = new global::System.Text.RegularExpressions.Regex(
            "\\((?<operator>define-only-symbolic)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords)))?\\s*\\)");
        foreach (int item in global::System.Linq.Enumerable.Range(symbolicEvalIndex, SymbolicEvalIndices[1]))
        {
            if (item >= aggregateRl.Count)
                return false;
            if (regex.Match(aggregateRl[item]).Success)
            {
                symbolicEvalIndex = item;
                return true;
            }
        }

        return false;
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

    private void HandleCalculatableAdded(
        global::Quire.Calculation.Core.ICalculatableContainer container,
        global::Quire.Calculation.Core.ICalculatable newCalc)
    {
        if (CalculatableAdded != null)
            CalculatableAdded(this, newCalc);
    }

    private void HandleCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable removedCalc)
    {
        if (CalculatableRemoved != null)
            CalculatableRemoved(removedCalc);
    }

    private void HandleCalculatableMoved(global::Quire.Calculation.Core.ICalculatable movedCalc)
    {
        if (CalculatableMoved != null)
            CalculatableMoved(movedCalc);
    }
}
