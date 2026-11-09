namespace Quire.Controls;

internal partial class SolverAggregateChildWrapper
{
    private readonly string _wrappedRlFormat = "(fragment {0})";

    public SolverAggregateChildWrapper(global::Quire.Calculation.Core.ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
    }

    public override string Rl
    {
        get
        {
            string wrappedRl;
            if (WrappedCalculatable is not global::Quire.Calculation.Core.ICalculatableAggregate aggregate ||
                global::Quire.EnumerableExtensions.IsNullOrEmpty(aggregate.CalculatableChildren))
                wrappedRl = WrappedCalculatable.Rl;
            else
                wrappedRl = SolveBlockCalculatableAggregate.ComposeSolveblockChildRlFromEquationControl(
                    global::Quire.Calculation.Core.CalculatableAggregateExtensions.GetAggregateRl(aggregate));

            return string.Format(_wrappedRlFormat, wrappedRl);
        }
    }

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
