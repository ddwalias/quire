namespace Quire.Controls;

internal partial class SolveBlockModel
{
    private void HandleSolverRequestEvaluation(global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        if (calculatable != Solver)
            throw new global::System.InvalidOperationException(
                "logic error: a handler only for the active solver got called by some other calculatable.");

        if (Solver.IsSolverOfSymbolicContainsFind(Solver.Rl) &&
            GetWrappedCalculatable(calculatable) is global::Quire.Controls.EquationControl
            {
                ContainsSymbolicEvaluation: not false
            } equationControl && equationControl.CalculatableChildren.Count() > 1)
        {
            Solver.AddChildAggregate(equationControl, this);
        }

        if (!string.IsNullOrEmpty(Solver.DefinedRl))
        {
            if (SolverResultDefinition == null)
            {
                SolverResultDefinition = new global::Quire.Calculation.Core.Support.SimpleCalculatable();
                FireCalculatableAdded(SolverResultDefinition);
            }

            // Release IL uses the literal format call instead of interpolated-string lowering.
            SolverResultDefinition.Rl = global::System.String.Format(
                "(define {0} {1})", SolverResultIdName, Solver.DefinedRl);
            FireSolverResultDefinitionEvaluation();
        }
        else
        {
            SolverIsNotInADefinition();
        }
    }
}
