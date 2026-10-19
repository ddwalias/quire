#nullable disable
using System.Collections.Generic;
using System.Windows.Threading;
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial class CalculatableWrapper
{
    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual ICalculatable WrappedCalculatable
    {
        get => _wrappedCalculatable;
        protected set
        {
            if (_wrappedCalculatable == value)
                return;
            UnhookCalculatableEvents(_wrappedCalculatable);
            _wrappedCalculatable = value;
            HookupCalculatableEvents(_wrappedCalculatable);
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual string Rl => WrappedCalculatable.Rl;

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public DispatcherPriority? ResultsProcessingPriority
    {
        get
        {
            if (!(WrappedCalculatable is ICalcWithPriority))
                return null;
            return (WrappedCalculatable as ICalcWithPriority).ResultsProcessingPriority;
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual bool ContainsSymbolicEvaluation
    {
        get
        {
            if (WrappedCalculatable is ICanContainSymbolicCalculation canContainSymbolicCalculation)
                return canContainSymbolicCalculation.ContainsSymbolicEvaluation;
            return false;
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public CalculatableWrapper(ICalculatable wrappedCalculatable)
    {
        _wrappedCalculatable = wrappedCalculatable;
        HookupCalculatableEvents(_wrappedCalculatable);
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    protected virtual IResultNode HandleGetValueOfIdentifier(ICalculatable calc, string idRl)
    {
        Func<ICalculatable, string, IResultNode> getValueOfIdentifierBefore = GetValueOfIdentifierBefore;
        if (getValueOfIdentifierBefore != null)
            return getValueOfIdentifierBefore(this, idRl);
        return null;
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    protected virtual void HandleTreatAsUpToDate(ICalculatable calc)
    {
        Action<ICalculatable> treatAsUpToDate = TreatAsUpToDate;
        if (treatAsUpToDate != null)
            treatAsUpToDate(this);
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    protected void FireRequestEvaluation(ICalculatable calc)
    {
        Action<ICalculatable> requestEvaluation = RequestEvaluation;
        if (requestEvaluation != null)
            requestEvaluation(this);
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual void Commit() => WrappedCalculatable.Commit();

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual void SetStatusSynchronized() => WrappedCalculatable.SetStatusSynchronized();

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual void SetSynchronizationStatus(SynchronizationStatus syncStatus) =>
        WrappedCalculatable.SetSynchronizationStatus(syncStatus);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual void SetCalculatingStatus(CalculatingStatus computingStatus) =>
        WrappedCalculatable.SetCalculatingStatus(computingStatus);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual void SetResult(IResultNode result) => WrappedCalculatable.SetResult(result);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual void SetError(EngineError error) => WrappedCalculatable.SetError(error);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
        Dictionary<ResolvedId, string[]> dictionary = new Dictionary<ResolvedId, string[]>();
        foreach (KeyValuePair<ResolvedId, string[]> item in idLabelMap)
        {
            ResolvedId key = item.Key;
            if (key.Calculatable == this)
            {
                key = new ResolvedId
                {
                    Calculatable = WrappedCalculatable,
                    Name = key.Name,
                    Tag = key.Tag,
                };
            }
            dictionary.Add(key, item.Value);
        }
        WrappedCalculatable.SetIdentifierLabels(dictionary);
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap) =>
        WrappedCalculatable.SetAutoSelectAlgorithm(autoAlgorithmMap);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings) =>
        WrappedCalculatable.SetRedefinitionWarnings(warnings);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    public virtual void Dispose() => UnhookCalculatableEvents(WrappedCalculatable);

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    private void HookupCalculatableEvents(ICalculatable calc)
    {
        if (calc == null)
            return;
        calc.RequestEvaluation += HandleRequestEvaluation;
        calc.GetValueOfIdentifierBefore += HandleGetValueOfIdentifier;
        calc.TreatAsUpToDate += HandleTreatAsUpToDate;
    }

    [MirrorVerified("PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests")]
    private void UnhookCalculatableEvents(ICalculatable calc)
    {
        if (calc == null)
            return;
        calc.RequestEvaluation -= HandleRequestEvaluation;
        calc.GetValueOfIdentifierBefore -= HandleGetValueOfIdentifier;
        calc.TreatAsUpToDate -= HandleTreatAsUpToDate;
    }
}
