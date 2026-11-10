namespace Quire.Controls;

internal partial class SolveBlockModel
{
    private void ListenForErrorChanges(global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        if (calculatable is not global::Quire.Controls.ISolveBlockCalculatable solveBlockCalculatable)
            return;
        if (solveBlockCalculatable.WrappedCalculatable is not global::Quire.Calculation.Core.ICalculatableInfo wrappedCalculatable)
            return;
        wrappedCalculatable.PropertyChanged += HandleWrappedCalcPropertyChanged;
    }

    private void StopListeningForErrorChanges(global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        if (calculatable is not global::Quire.Controls.ISolveBlockCalculatable solveBlockCalculatable)
            return;
        if (solveBlockCalculatable.WrappedCalculatable is not global::Quire.Calculation.Core.ICalculatableInfo wrappedCalculatable)
            return;
        wrappedCalculatable.PropertyChanged -= HandleWrappedCalcPropertyChanged;
    }

    private void HandleEquationControlCommitted(global::Quire.Calculation.Core.ICalculatable calc)
    {
        // Release IL branches directly to the removal body after the shared early return.
        if (calc != null && calc.Equals(Solver?.WrappedCalculatable))
            goto RemoveChild;
        return;

    RemoveChild:
        Solver?.RemoveChild(calc);
    }
}
