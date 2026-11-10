using System.Collections.Generic;

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

    public string DefinedRl { get; private set; }
    public string DefinedParams { get; private set; }
    private string SolverName { get; set; }
    private string SolverTag { get; set; }
    private string[] SolverArgs { get; set; }
    private string LeftOfSolverApply { get; set; }
    private string RightOfSolverApply { get; set; }

    public override string Rl => _rl;
    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren =>
        _calculatableChildren;

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

    public SolverAggregate(global::Quire.Calculation.Core.ICalculatable calculatable)
        : base(calculatable)
    {
        _calculatableChildren = new List<global::Quire.Calculation.Core.ICalculatable>();
        EquationType = SolveBlockEquationTypes.Solver;
    }

    public bool IsSolverOfSymbolicContainsUndefineSubscripts(string rl)
    {
        return rl.Contains("undefine-subscripts");
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
