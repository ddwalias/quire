using System.Collections.Generic;

namespace Quire.Controls;

internal partial class SolveBlockModel
{
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
}
