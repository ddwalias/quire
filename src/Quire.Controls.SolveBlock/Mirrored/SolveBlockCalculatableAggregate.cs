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

    private static string ComposeRlForSymEval(
        global::System.Collections.Generic.IList<string> aggregateRl, int symbolicEvalIndex)
    {
        var symbolicRegex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>define-only-symbolic)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords)))?\\s*\\)");
        var symbolicMatch = symbolicRegex.Match(aggregateRl[symbolicEvalIndex]);
        var defineRegex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>define)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*\\)");
        var defineMatch = defineRegex.Match(aggregateRl[0]);
        if (defineMatch.Success && aggregateRl.Count == 3)
        {
            string definedVariable = GetDefinedVariable(defineMatch);
            string expression = GetSymbolicEvalExpression(symbolicMatch);
            string operatorTag = defineMatch.Groups["operatorTag"].Value;
            string keywords = symbolicMatch.Groups["keywords"].Value;
            if (global::Quire.EnumerableExtensions.IsNotEmpty<char>(definedVariable) &&
                global::Quire.EnumerableExtensions.IsNotEmpty<char>(expression) &&
                global::Quire.EnumerableExtensions.IsNotEmpty<char>(operatorTag) &&
                global::Quire.EnumerableExtensions.IsNotEmpty<char>(keywords))
            {
                var builder = new global::System.Text.StringBuilder();
                builder.Append('(').Append("define-symeval").Append(operatorTag).Append(' ')
                    .Append(definedVariable).Append(' ').Append(expression).Append(' ').Append(keywords).Append(')');
                return builder.ToString();
            }
        }
        else if (symbolicMatch.Success)
        {
            string expression = GetSymbolicEvalExpression(symbolicMatch);
            string operatorTag = symbolicMatch.Groups["operatorTag"].Value;
            string operatorName = symbolicMatch.Groups["operator"].Value;
            string resultId = symbolicMatch.Groups["resultId"].Value;
            string keywords = symbolicMatch.Groups["keywords"].Value;
            if (global::Quire.EnumerableExtensions.IsNotEmpty<char>(operatorName) &&
                global::Quire.EnumerableExtensions.IsNotEmpty<char>(expression) &&
                global::Quire.EnumerableExtensions.IsNotEmpty<char>(operatorTag) &&
                global::Quire.EnumerableExtensions.IsNotEmpty<char>(resultId) &&
                global::Quire.EnumerableExtensions.IsNotEmpty<char>(keywords))
            {
                var builder = new global::System.Text.StringBuilder();
                builder.Append('(').Append(operatorName).Append(operatorTag).Append(' ')
                    .Append(resultId).Append(' ').Append(expression).Append(' ').Append(keywords).Append(')');
                return builder.ToString();
            }
        }

        return null;
    }

    private static string ComposeRlForEvalOrDefineEval(
        global::System.Collections.Generic.IList<string> aggregateRl)
    {
        var regex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>eval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<scaleFactor>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-scaleFactor)))?\\s*\\)");
        var defineRegex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>define)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*\\)");
        var defineMatch = defineRegex.Match(aggregateRl[0]);
        var defineMatchForEval = defineRegex.Match(aggregateRl[1]);
        var defineMatchForScale = defineRegex.Match(aggregateRl[2]);
        var evalMatch = regex.Match(aggregateRl[3]);
        if ((!defineMatch.Success && aggregateRl[0] != "nil") || !defineMatchForEval.Success ||
            (!defineMatchForScale.Success && aggregateRl[2] != "nil") || !evalMatch.Success)
        {
            return null;
        }

        var stringBuilder = new global::System.Text.StringBuilder();
        stringBuilder.Append('(');
        if (defineMatch.Success)
        {
            if (aggregateRl.Count != 5 || !regex.IsMatch(aggregateRl[4]))
                return null;

            string operatorTag = defineMatch.Groups["operatorTag"].Value;
            string definedVariable = GetDefinedVariable(defineMatch);
            stringBuilder.Append("define-eval").Append(operatorTag).Append(' ')
                .Append(definedVariable);
        }
        else
        {
            if (aggregateRl.Count != 4)
                return null;

            string operatorTag = evalMatch.Groups["operatorTag"].Value;
            stringBuilder.Append("eval").Append(operatorTag);
        }

        string expression = defineMatchForEval.Groups["expression"].Value;
        stringBuilder.Append(' ').Append(expression);
        if (defineMatchForScale.Success)
        {
            string defined = defineMatchForScale.Groups["defined"].Value;
            string scaleExpression = defineMatchForScale.Groups["expression"].Value;
            if (defined != "ui_scale")
                return null;

            stringBuilder.Append(' ').Append(scaleExpression);
        }

        stringBuilder.Append(')');
        return stringBuilder.ToString();
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

    internal static string ComposeSolveblockChildRlFromEquationControl(
        global::System.Collections.Generic.IList<string> aggregateRl)
    {
        if (IsRlForSymbolicEval(aggregateRl, out int symbolicEvalIndex))
            return ComposeRlForSymEval(aggregateRl, symbolicEvalIndex);
        if (aggregateRl.Count == 4 || aggregateRl.Count == 5)
            return ComposeRlForEvalOrDefineEval(aggregateRl);
        return null;
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


