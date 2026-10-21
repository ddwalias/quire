#nullable disable
using System;
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial class SimpleCalculatable
{
    private CalculatingStatus _calculatingStatus = (CalculatingStatus)2;

    private IResultNode _evaluationResult = new EmptySimpleResultNode();

    private EngineError _error;

    protected RealSimpleResultNode _systemOrigin = new RealSimpleResultNode
    {
        Value = 0.0
    };

    private IList<EngineRedefInfo> _warnings;

    private SynchronizationStatus _syncStatus;

    public SimpleCalculatable()
    {
    }

    public virtual void Commit()
    {
        _error = null;
        _evaluationResult = new EmptySimpleResultNode();
        if (RequestEvaluation != null)
        {
            RequestEvaluation(this);
        }
    }

    public void SetStatusSynchronized()
    {
        if (TreatAsUpToDate != null)
        {
            TreatAsUpToDate(this);
        }
    }

    public virtual void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        _syncStatus = syncStatus;
        if (syncStatus == (SynchronizationStatus)3 && Synchronized != null)
        {
            Synchronized(this);
        }
    }

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetCalculatingStatus(CalculatingStatus computingStatus) => _calculatingStatus = computingStatus;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetResult(IResultNode result) => _evaluationResult = result;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetError(EngineError error) => _error = error;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap) { }

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings) => _warnings = warnings;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap) { }

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual IResultNode Result => _evaluationResult;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public IList<EngineRedefInfo> EngineWarnings => _warnings;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual SynchronizationStatus SynchronizationStatus => _syncStatus;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual CalculatingStatus CalculatingStatus => _calculatingStatus;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual EngineError Error => _error;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public virtual string Rl { get; set; }

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public RealSimpleResultNode SystemOrigin => _systemOrigin;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public event Action<ICalcWithSyncStatus> Synchronized;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public event Action<ICalculatable> RequestEvaluation;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    [MirrorVerified("PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests")]
    public event Action<ICalculatable> TreatAsUpToDate;

    public virtual bool IsErrorOnScalingUnits()
    {
        DebugTools.FailAssertion("IsErrorOnScalingUnits is not implemented");
        return false;
    }

    public static bool IsEngineWarningsEquals(IList<EngineRedefInfo> cacheWarningsOrOther, IList<EngineRedefInfo> newWarnings)
    {
        if (cacheWarningsOrOther == null && newWarnings == null)
        {
            return true;
        }
        if (cacheWarningsOrOther == null)
        {
            return false;
        }
        if (newWarnings == null)
        {
            return false;
        }
        if (cacheWarningsOrOther.Count != newWarnings.Count)
        {
            return false;
        }
        for (int i = 0; i < cacheWarningsOrOther.Count; i++)
        {
            if (cacheWarningsOrOther[i].Name != newWarnings[i].Name || cacheWarningsOrOther[i].OldLocation != newWarnings[i].OldLocation || !cacheWarningsOrOther[i].OldType.Equals(newWarnings[i].OldType))
            {
                return false;
            }
        }
        return true;
    }

    public IResultNode FireGetValueOfIdentifierBefore(string idRl)
    {
        if (GetValueOfIdentifierBefore != null)
        {
            return GetValueOfIdentifierBefore(this, idRl);
        }
        return null;
    }
}
