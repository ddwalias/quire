using System.Collections.Generic;

namespace Quire.Controls;

internal partial class SolveBlockModel
{
    private readonly global::System.Collections.Generic.IDictionary<global::Quire.Calculation.Core.ICalculatable, global::Quire.Controls.ISolveBlockCalculatable> _solveBlockCalculatableMap =
        new global::System.Collections.Generic.Dictionary<global::Quire.Calculation.Core.ICalculatable, global::Quire.Controls.ISolveBlockCalculatable>();
    private global::Quire.Controls.ISolverAggregate _solver;

    private global::Quire.Controls.SolveBlockChildManager SBChildManager =>
        (global::Quire.Controls.SolveBlockChildManager)_childManager;

    public global::Quire.Controls.Core.IWhiteboardItem SolverItem { get; private set; }
    private global::Quire.Calculation.Core.Support.SimpleCalculatable SolverResultDefinition { get; set; }
    public global::System.Collections.Generic.IEnumerable<global::Quire.Controls.ISolveBlockCalculatable> AllCalculatables =>
        _solveBlockCalculatableMap.Values;
    public global::System.Windows.ResourceDictionary Resources { get; set; }

    internal global::Quire.Controls.ISolverAggregate Solver
    {
        get => _solver;
        private set
        {
            _solver = value;
            SolverItem = _solver != null ? GetWhiteboardItemThatOwnsCalculatable(Solver) : null;
        }
    }

    public SolveBlockModel()
        : base(null)
    {
    }

    private const uint GuessValuesSectionIndex = 0;
    private const uint ConstraintSectionIndex = 1;
    private const uint SolverSectionIndex = 2;
    private const int SectionCount = 3;

    public override object Clone()
    {
        var clone = new SolveBlockModel();
        CopyStateTo(clone);
        return clone;
    }

    protected override global::Quire.Calculation.Core.ICalculatable GetWrappedCalculatable(
        global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        if (!(calculatable is global::Quire.Controls.ISolveBlockCalculatable))
            return calculatable;
        return (calculatable as global::Quire.Controls.ISolveBlockCalculatable).WrappedCalculatable;
    }

    protected override void AddItems(IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items)
    {
        base.AddItems(items);
        UpdateWhichItemIsSolver();
    }

    protected override void RemoveItems(IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items)
    {
        base.RemoveItems(items);
        UpdateWhichItemIsSolver();
    }

    public override int BlockSectionCount => SectionCount;

    public override IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GetBlockSectionItems(uint sectionIdx)
    {
        switch (sectionIdx)
        {
            case GuessValuesSectionIndex:
                return GuessValueItems;
            case ConstraintSectionIndex:
                return ConstraintItems;
            case SolverSectionIndex:
                if (SolverItem != null)
                    return new global::Quire.Controls.Core.IWhiteboardItem[] { SolverItem };
                return null;
            default:
                throw new global::System.IndexOutOfRangeException("solve block contains 3 sections");
        }
    }

    public override string GetBlockSectionName(uint sectionIdx)
    {
        // Release IL returns directly from each case rather than sharing a switch-expression return.
        switch (sectionIdx)
        {
            case GuessValuesSectionIndex:
                return (string)Resources["SolveBlock.GuessValues.Text.Key"];
            case ConstraintSectionIndex:
                return (string)Resources["SolveBlock.Constraints.Text.Key"];
            case SolverSectionIndex:
                return (string)Resources["SolveBlock.Solver.Text.Key"];
            default:
                throw new global::System.IndexOutOfRangeException("solve block contains 3 sections");
        }
    }

    public override double GetBlockSectionMinHeightRatio(uint sectionIdx)
    {
        switch (sectionIdx)
        {
            case GuessValuesSectionIndex:
            case ConstraintSectionIndex:
                return 0.36;
            case SolverSectionIndex:
                return 0.18;
            default:
                throw new global::System.IndexOutOfRangeException("solve block contains 3 sections");
        }
    }

    private static bool IsSolverAggregateChildType(SolveBlockEquationTypes eqType)
    {
        if (eqType != SolveBlockEquationTypes.Definition)
            return eqType == SolveBlockEquationTypes.Constraint;
        return true;
    }

    private static bool IsSolverAggregateChildType(global::Quire.Calculation.Core.ICalculatable modelCalcChild)
    {
        global::Quire.Controls.ISolveBlockCalculatable wrapper =
            modelCalcChild as global::Quire.Controls.ISolveBlockCalculatable;
        SolveBlockEquationTypes equationType;
        if (wrapper != null)
            equationType = wrapper.EquationType;
        else
            equationType = SolveBlockCalculatable.GetEquationTypeOfCalc(modelCalcChild);
        return IsSolverAggregateChildType(equationType);
    }

    public SolveBlockEquationTypes GetSolveBlockEquationType(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        if (GuessValueItems.Contains(item))
            return SolveBlockEquationTypes.Definition;
        if (ConstraintItems.Contains(item))
            return SolveBlockEquationTypes.Constraint;
        if (SolverItem != null && SolverItem.Equals(item))
            return SolveBlockEquationTypes.Solver;
        return SolveBlockEquationTypes.Unknown;
    }

    public string GetBlockEquationTypeAttributeName(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        switch (GetSolveBlockEquationType(item))
        {
            case SolveBlockEquationTypes.Constraint:
                return global::Quire.PersistentDataObjects.solveBlockRegionTypeSolveblockcategory.constraint.ToString();
            case SolveBlockEquationTypes.Definition:
                return global::Quire.PersistentDataObjects.solveBlockRegionTypeSolveblockcategory.guessvalue.ToString();
            case SolveBlockEquationTypes.Solver:
                return global::Quire.PersistentDataObjects.solveBlockRegionTypeSolveblockcategory.solver.ToString();
            default:
                return null;
        }
    }

    protected override global::Quire.Calculation.Core.Support.CalculatableChildHider InitializeChildManager()
    {
        return new SolveBlockChildManager(
            WhiteboardItems,
            FireCalculatableAdded,
            FireCalculatableRemoved,
            FireCalculatableMoved,
            CreateCalculatableWrapper,
            LookupCalculatableWrapper,
            RemoveCalculatableWrapper,
            HandleChildCalcHiddenStateChanged,
            HandleContainerChildrenChanged);
    }

    private global::Quire.Controls.Core.IWhiteboardItem GetWhiteboardItemThatOwnsCalculatable(
        global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        global::Quire.Calculation.Core.ICalculatable wrappedCalculatable = GetWrappedCalculatable(calculatable);
        foreach (global::Quire.Controls.Core.IWhiteboardItem whiteboardItem in WhiteboardItems)
        {
            if (whiteboardItem == wrappedCalculatable)
                return whiteboardItem;
            if (whiteboardItem is global::Quire.Calculation.Core.ICalculatableContainer container &&
                !(container is global::Quire.Calculation.Core.ICalculatableAggregate) &&
                container.CalculatableChildren.Contains(wrappedCalculatable))
                return whiteboardItem;
        }

        return null;
    }
}
