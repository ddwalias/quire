namespace Quire.Controls;

internal partial class SolverAggregateChildWrapper
{
    public bool ContainsError(global::Quire.Calculation.Core.EngineError error)
    {
        return error.ErrorSource == this;
    }

    public void ShowError(global::Quire.Calculation.Core.EngineError error)
    {
        if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationErrorOwner owner)
            owner.ShowError(error);
    }
}
