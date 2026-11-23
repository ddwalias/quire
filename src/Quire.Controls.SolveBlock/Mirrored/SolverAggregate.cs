using System.Collections.Generic;
using System.Linq;

namespace Quire.Controls;

internal partial class SolverAggregate
{
    private const string OdeDefaultAlgorithmString = "adams";

    private const string PdeDefaultAlgorithmString = "0";

    private static Dictionary<string, string> PdeAlgoToRL = new Dictionary<string, string>
    {
        { "", "0" },
        { "polynomial", "1" },
        { "centralDifferences", "2" },
        { "5PointDifferences", "3" }
    };

    private static Dictionary<string, string> SolvingAlgoToRL = new Dictionary<string, string>
    {
        { "", "smart" },
        { "linear", "linear" },
        { "nonlinearLevenbergMarquardt", "levmar" },
        { "nonlinearConjugateGradient", "conjgrad" },
        { "nonlinearSQP", "sqp" },
        { "nonlinearInteriorPoint", "interpoint" },
        { "nonlinearActiveSet", "activeset" }
    };

    private static Dictionary<string, string> SolvingAlgoOptionsToRL = new Dictionary<string, string>
    {
        { "multiStart", "mstart" },
        { "optimize", "optim" }
    };

    private const string SolverFind = "find";

    private const string Minerr = "minerr";

    private const string Pdesolve = "pdesolve";

    private const string Odesolve = "odesolve";

    private const string PrognExp = "(progn:0x0000ffff ,%!)";

    private const string LambdaExp = "lambda";

    public const string RL_NIL = "nil";

    private const int DefaultEvalIndex = 2;

    private const string UndefineSubscripts = "undefine-subscripts";

    private string _rl;

    private readonly List<global::Quire.Calculation.Core.ICalculatable> _calculatableChildren;

    public SolverAggregate(global::Quire.Calculation.Core.ICalculatable calculatable)
        : base(calculatable)
    {
        _calculatableChildren = new List<global::Quire.Calculation.Core.ICalculatable>();
        EquationType = SolveBlockEquationTypes.Solver;
    }

    public override string Rl => _rl;

    // Placed before the auto-properties: the lambda ordinal must be DisplayClass19_0 as in Release IL.
    public void UpdateSolverRL()
    {
        if (SolverName == null && WrappedCalculatable?.Rl != null)
            ParseWrappedRl();
        if (global::Quire.DebugTools.AssertNotNull(SolverName, null).Failed)
            _rl = "nil";
        string text = char.ToLowerInvariant(SolverName[0]) + SolverName.Substring(1);
        bool flag = text == "odesolve";
        bool flag2 = text == "pdesolve";
        global::System.Text.StringBuilder solverRlBuilder = new global::System.Text.StringBuilder();
        solverRlBuilder.Append(LeftOfSolverApply);
        if (DefinedParams.IsNotEmpty())
        {
            if (DefinedParams.Contains(PrognExp) && DefinedParams.Length > PrognExp.Length)
                DefinedParams = DefinedParams.Substring(0, DefinedParams.Length - PrognExp.Length - 1);
            solverRlBuilder.Append(DefinedParams);
        }
        if (!flag && !flag2)
            solverRlBuilder.Append("(subscript ");
        solverRlBuilder.Append("(solveblock").Append(SolverTag).Append(" %@ (")
            .Append(text)
            .Append(SolverTag);
        AddAlgorithmIfRequired(solverRlBuilder, WrappedCalculatable, SolverName);
        SolverArgs.Each((string arg) =>
        {
            solverRlBuilder.Append(" ").Append(arg);
        });
        solverRlBuilder.Append("))");
        if (!flag && !flag2)
            solverRlBuilder.Append(" mc_ORIGIN)");
        if (DefinedParams.IsNotEmpty())
            solverRlBuilder.Append(")");
        solverRlBuilder.Append(RightOfSolverApply);
        _rl = solverRlBuilder.ToString();
    }

    // Placed before the auto-properties: the lambda ordinal must be b__20_0 as in Release IL.
    private static string GetSolvingAlgorithmRl(string algorithmString)
    {
        if (string.IsNullOrEmpty(algorithmString))
            return SolvingAlgoToRL[string.Empty];
        string[] array = algorithmString.Split('-');
        string text = SolvingAlgoToRL.TryGetValue(array[0], out var value) ? value : SolvingAlgoToRL[string.Empty];
        string text2 = string.Join(" ", from key in array.Skip(1)
            where SolvingAlgoOptionsToRL.ContainsKey(key)
            select SolvingAlgoOptionsToRL[key]);
        if (!string.IsNullOrEmpty(text2))
            return text + " " + text2;
        return text;
    }

    private void AddAlgorithmIfRequired(
        global::System.Text.StringBuilder solverRlBuilder,
        global::Quire.Calculation.Core.ICalculatable wrappedCalculatable,
        string solverName)
    {
        string pattern = global::System.Text.RegularExpressions.Regex.Escape(solverName) +
            "\\s+\\(@ALGO\\s+([\\w-]+)\\)";
        string algorithm = string.Empty;
        if (WrappedCalculatable is global::Quire.Controls.EquationControl equationControl)
        {
            global::System.Text.RegularExpressions.Match match =
                global::System.Text.RegularExpressions.Regex.Match(equationControl.EquationSExpression, pattern);
            if (match != null && match.Success)
                algorithm = match.Groups[1].Value;
        }

        switch (solverName.ToLower())
        {
            case "find":
            case "minerr":
                if (algorithm != string.Empty)
                    solverRlBuilder.Append(" ").Append(GetSolvingAlgorithmRl(algorithm));
                break;
            case "odesolve":
                if (algorithm != string.Empty)
                    solverRlBuilder.Append(" ").Append(algorithm);
                else
                    solverRlBuilder.Append(" ").Append(OdeDefaultAlgorithmString);
                break;
            case "pdesolve":
                if (algorithm != string.Empty)
                    solverRlBuilder.Append(" ").Append(PdeAlgoToRL[algorithm]);
                else
                    solverRlBuilder.Append(" ").Append(PdeDefaultAlgorithmString);
                break;
        }
    }

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> Children =>
        CalculatableChildren.Cast<global::Quire.Controls.SolverAggregateChildWrapper>()
            .Collect((global::Quire.Controls.SolverAggregateChildWrapper calc) => calc.WrappedCalculatable);

    public global::Quire.Calculation.Core.ICalculatable LastChild
    {
        get
        {
            int count = _calculatableChildren.Count;
            if (count == 0)
                return null;
            return ((global::Quire.Controls.SolverAggregateChildWrapper)_calculatableChildren[count - 1])
                .WrappedCalculatable;
        }
    }

    public string DefinedRl { get; private set; }

    public string DefinedParams { get; private set; }

    private string SolverName { get; set; }

    private string SolverTag { get; set; }

    private string[] SolverArgs { get; set; }

    private string LeftOfSolverApply { get; set; }

    private string RightOfSolverApply { get; set; }

    public bool IsSolverOfSymbolicContainsFind(string rl)
    {
        var regex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>define-only-symbolic)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords)))?\\s*\\)");
        var match = regex.Match(rl);
        if (!match.Success)
        {
            regex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>define-symeval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords)))?\\s*\\)");
            match = regex.Match(rl);
        }

        if (match.Success && match.Groups["expression"].Value.IndexOf(
                "find", global::System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return true;
        }

        return false;
    }

    public bool IsSolverOfSymbolicContainsUndefineSubscripts(string rl)
    {
        return rl.Contains("undefine-subscripts");
    }

    private global::Quire.Controls.SolverAggregateChildWrapperAggregate GetSolverWrappedAggregate()
    {
        if (CalculatableChildren.First((global::Quire.Calculation.Core.ICalculatable child) => IsSolverOfSymbolicContainsFind(child.Rl))
            is global::Quire.Controls.SolverAggregateChildWrapperAggregate result)
            return result;
        return null;
    }

    protected override void HandleRequestEvaluation(global::Quire.Calculation.Core.ICalculatable calc)
    {
        // Release IL branches to the shared terminal return when the equation type changes.
        if (UpdateEquationType(WrappedCalculatable))
            goto Finished;

        ParseWrappedRl();
        UpdateSolverRL();
        FireRequestEvaluation(this);
        if (IsSolverOfSymbolicContainsFind(Rl))
        {
            if (WrappedCalculatable is global::Quire.Controls.EquationControl
                {
                    ContainsSymbolicEvaluation: not false
                } equationControl)
            {
                if (equationControl.CalculatableChildren.Count() > 1)
                {
                    SolverAggregateChildWrapperAggregate solverWrappedAggregate = GetSolverWrappedAggregate();
                    if (solverWrappedAggregate != null)
                    {
                        foreach (global::Quire.Calculation.Core.ICalculatable child in solverWrappedAggregate.CalculatableChildren)
                        {
                            if (!IsSolverOfSymbolicContainsUndefineSubscripts(child.Rl) &&
                                !IsSolverOfSymbolicContainsFind(child.Rl))
                            {
                                child.Commit();
                            }
                        }
                    }
                }
            }
        }

    Finished:
        return;
    }

    protected override bool UpdateEquationType(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (!(WrappedCalculatable is global::Quire.Calculation.Core.ICalculatableAggregate aggregate) ||
            aggregate.CalculatableChildren.IsNullOrEmpty())
            return base.UpdateEquationType(calc);
        return UpdateEquationTypeOfAggregateCalculatable(aggregate);
    }

    protected bool UpdateEquationTypeOfAggregateCalculatable(
        global::Quire.Calculation.Core.ICalculatableAggregate calc)
    {
        SolveBlockEquationTypes oldEquationType = EquationType;
        EquationType = SolveBlockCalculatable.GetEquationTypeOfCalc(calc);
        return EquationType != oldEquationType;
    }

    public bool IsChild(global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        return CalculatableChildren.Cast<global::Quire.Controls.SolverAggregateChildWrapper>()
            .Any((global::Quire.Controls.SolverAggregateChildWrapper wrapper) => wrapper.WrappedCalculatable == calculatable);
    }

    public int GetIndexOfChild(global::Quire.Calculation.Core.ICalculatable child)
    {
        if (child == null)
            return -1;
        return _calculatableChildren.IndexOf(_calculatableChildren.Cast<global::Quire.Controls.SolverAggregateChildWrapper>()
            .FirstOrDefault((global::Quire.Controls.SolverAggregateChildWrapper calc) => calc.WrappedCalculatable == child));
    }

    public void CommitAllChildren()
    {
        Children.Each((global::Quire.Calculation.Core.ICalculatable child) =>
        {
            child.Commit();
        });
    }

    public void AddChild(global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        var wrapper = new global::Quire.Controls.SolverAggregateChildWrapper(calculatable);
        _calculatableChildren.Add(wrapper);
        FireCalculatableAdded(wrapper, null);
    }

    public void AddChildAggregate(
        global::Quire.Calculation.Core.ICalculatable calculatable,
        global::Quire.Calculation.Core.ICalculatableContainer container)
    {
        var wrapper = new global::Quire.Controls.SolverAggregateChildWrapperAggregate(calculatable);
        _calculatableChildren.Add(wrapper);
        FireCalculatableAdded(wrapper, container);
    }

    public void InsertChildBefore(
        global::Quire.Calculation.Core.ICalculatable insertBefore,
        global::Quire.Calculation.Core.ICalculatable calcToInsert)
    {
        if (insertBefore == null)
        {
            AddChild(calcToInsert);
            return;
        }

        int index = GetIndexOfChild(insertBefore);
        if (index == -1)
            throw new global::System.InvalidOperationException(
                "logic error: The calculatable that should follow the child to be inserted is not currently a child of the solver");

        var wrapper = new global::Quire.Controls.SolverAggregateChildWrapper(calcToInsert);
        _calculatableChildren.Insert(index, wrapper);
        FireCalculatableAdded(wrapper, this);
    }

    public void RemoveChild(global::Quire.Calculation.Core.ICalculatable calcToRemove)
    {
        int index = GetIndexOfChild(calcToRemove);
        // Release IL branches to removal on a valid index and falls through to return otherwise.
        if (index != -1)
            goto Remove;
        return;

    Remove:
        var wrapper = (global::Quire.Controls.SolverAggregateChildWrapper)_calculatableChildren[index];
        _calculatableChildren.Remove(wrapper);
        FireCalculatableRemoved(wrapper);
        wrapper.Dispose();
    }

    public void MoveChildBefore(
        global::Quire.Calculation.Core.ICalculatable moveBefore,
        global::Quire.Calculation.Core.ICalculatable calcToMove)
    {
        int index = GetIndexOfChild(calcToMove);
        if (index == -1)
            throw new global::System.InvalidOperationException(
                "logic error: The calculatable to be moved is not currently a child of the solver");

        var wrapper = (global::Quire.Controls.SolverAggregateChildWrapper)_calculatableChildren[index];
        _calculatableChildren.RemoveAt(index);
        if (moveBefore == null)
        {
            _calculatableChildren.Add(wrapper);
        }
        else
        {
            int nextIndex = GetIndexOfChild(moveBefore);
            if (nextIndex == -1)
                throw new global::System.InvalidOperationException(
                    "logic error: The calculatable that should follow the moved child is not currently a child of the solver");
            _calculatableChildren.Insert(nextIndex, wrapper);
        }

        FireCalculatableMoved(wrapper);
    }

    public override void SetSynchronizationStatus(global::Quire.Calculation.Core.SynchronizationStatus syncStatus)
    {
        base.SetSynchronizationStatus(syncStatus);
    }

    public override void SetCalculatingStatus(global::Quire.Calculation.Core.CalculatingStatus computingStatus)
    {
        base.SetCalculatingStatus(computingStatus);
    }

    private static int GetChildIndexForResult(global::Quire.Calculation.Core.ICalculatableContainer aggregate)
    {
        for (int index = 0; index < aggregate.CalculatableChildren.Count(); index++)
        {
            // Release IL calls Regex.Match virtually through this local.
            var regex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>define-only-symbolic)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords)))?\\s*\\)");
            if (regex.Match(aggregate.CalculatableChildren.ElementAt(index).Rl).Success)
            {
                return index;
            }
        }

        return 2;
    }

    public override void SetResult(global::Quire.Calculation.Core.IResultNode result)
    {
        if (!(WrappedCalculatable is global::Quire.Calculation.Core.ICalculatableAggregate aggregate) ||
            aggregate.CalculatableChildren.IsNullOrEmpty())
        {
            base.SetResult(result);
            return;
        }

        int childIndex = GetChildIndexForResult(aggregate);
        if (global::Quire.DebugTools.DevAssert(
                childIndex >= 0 && childIndex < aggregate.CalculatableChildren.Count(), null).Passed)
        {
            aggregate.CalculatableChildren.ElementAt(childIndex).SetResult(result);
        }
    }

    public override void SetError(global::Quire.Calculation.Core.EngineError error)
    {
        if (error == null)
        {
            ClearError();
            return;
        }

        global::Quire.Calculation.Core.ICalculatable errorSource = error.ErrorSource;
        if (errorSource == this || errorSource == null)
        {
            ClearError();
            base.SetError(error);
        }
        else if (_calculatableChildren.Contains(errorSource))
        {
            ClearError();
            errorSource.SetError(error);
        }
    }

    private void ClearError()
    {
        base.SetError(null);
        _calculatableChildren.Each((global::Quire.Calculation.Core.ICalculatable child) => child.SetError(null));
    }

    private void ParseWrappedRl()
    {
        string input;
        global::System.Collections.Generic.IList<string> aggregateRl = null;
        var calculatableAggregate = WrappedCalculatable as global::Quire.Calculation.Core.ICalculatableAggregate;
        bool containsSymbolicEvaluation = calculatableAggregate?.ContainsSymbolicEvaluation ?? false;
        if (calculatableAggregate == null ||
            global::Quire.EnumerableExtensions.IsNullOrEmpty<global::Quire.Calculation.Core.ICalculatable>(
                calculatableAggregate.CalculatableChildren))
        {
            input = WrappedCalculatable.Rl;
        }
        else
        {
            aggregateRl = global::Quire.Calculation.Core.CalculatableAggregateExtensions.GetAggregateRl(
                calculatableAggregate);
            input = SolveBlockCalculatableAggregate.ComposeSolveblockChildRlFromEquationControl(aggregateRl);
        }

        var match = SolveBlockCalculatable._solverMatch.Match(input);
        DefinedParams = string.Empty;
        DefinedRl = match.Groups["defined"].Value;
        if ((aggregateRl != null) & containsSymbolicEvaluation)
        {
            // Locals force the virtual Regex.Match calls used by Release IL.
            var defineRegex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>define)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*\\)");
            var defineMatch = defineRegex.Match(aggregateRl[0]);
            if (defineMatch.Success)
                DefinedRl = SolveBlockCalculatableAggregate.GetDefinedVariable(defineMatch);

            string expression = defineMatch.Groups["expression"].Value;
            if (expression.Contains("lambda"))
            {
                var symbolicRegex = new global::System.Text.RegularExpressions.Regex("\\((?<operator>define-symeval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords)))?\\s*\\)");
                if (symbolicRegex.Match(input).Success)
                    DefinedParams = expression;
            }
        }

        SolverName = match.Groups["solver_name"].Value;
        SolverTag = match.Groups["solver_tag"].Value;
        SolverArgs = new string[match.Groups["solver_arg"].Captures.Count];
        int index = 0;
        foreach (global::System.Text.RegularExpressions.Capture capture in match.Groups["solver_arg"].Captures)
            SolverArgs[index++] = capture.Value;
        LeftOfSolverApply = match.Groups["left_of_solver"].Value;
        RightOfSolverApply = match.Groups["right_of_solver"].Value;
    }

    public override bool ContainsError(global::Quire.Calculation.Core.EngineError error)
    {
        if (error.ErrorSource == this)
            return true;
        return CalculatableChildren.OfType<global::Quire.Calculation.Core.ICalculationErrorOwner>()
            .Any((global::Quire.Calculation.Core.ICalculationErrorOwner owner) => owner.ContainsError(error));
    }

    public override void ShowError(global::Quire.Calculation.Core.EngineError error)
    {
        global::Quire.Calculation.Core.ICalculationErrorOwner errorOwner;
        if (error.ErrorSource == this)
            errorOwner = WrappedCalculatable as global::Quire.Calculation.Core.ICalculationErrorOwner;
        else
            errorOwner = CalculatableChildren.OfType<global::Quire.Calculation.Core.ICalculationErrorOwner>()
                .FirstOrDefault((global::Quire.Calculation.Core.ICalculationErrorOwner owner) => owner.ContainsError(error));
        errorOwner?.ShowError(error);
    }

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableMoved;

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren =>
        _calculatableChildren;

    private void FireCalculatableAdded(
        global::Quire.Controls.SolverAggregateChildWrapper wrapper,
        global::Quire.Calculation.Core.ICalculatableContainer container)
    {
        if (CalculatableAdded != null)
            CalculatableAdded(container ?? this, wrapper);
        if (!wrapper.ContainsSymbolicEvaluation)
            wrapper.Commit();
    }

    private void FireCalculatableRemoved(global::Quire.Controls.SolverAggregateChildWrapper wrapper)
    {
        if (CalculatableRemoved != null)
            CalculatableRemoved(wrapper);
    }

    private void FireCalculatableMoved(global::Quire.Controls.SolverAggregateChildWrapper wrapper)
    {
        if (CalculatableMoved != null)
            CalculatableMoved(wrapper);
    }
}
