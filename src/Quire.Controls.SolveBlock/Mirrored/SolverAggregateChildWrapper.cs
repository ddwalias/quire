namespace Quire.Controls;

internal partial class SolverAggregateChildWrapper
{
    public global::Quire.Calculation.Core.EngineError CurrentError
    {
        get
        {
            if (WrappedCalculatable is global::Quire.Calculation.Core.ICalculationErrorOwner owner)
                return owner.CurrentError;
            return null;
        }
    }

    public override void SetSynchronizationStatus(global::Quire.Calculation.Core.SynchronizationStatus syncStatus)
    {
    }

    public override void Commit()
    {
        FireRequestEvaluation(this);
    }

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
