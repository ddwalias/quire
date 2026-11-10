using System.Collections.Generic;

namespace Quire.Controls;

internal partial class SolverAggregate
{
    private readonly List<global::Quire.Calculation.Core.ICalculatable> _calculatableChildren;

    public string DefinedRl { get; private set; }
    public string DefinedParams { get; private set; }
    private string SolverName { get; set; }
    private string SolverTag { get; set; }
    private string[] SolverArgs { get; set; }
    private string LeftOfSolverApply { get; set; }
    private string RightOfSolverApply { get; set; }

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
}
