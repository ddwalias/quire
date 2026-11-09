#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;

namespace Quire.Controls.Mathboard;

public abstract partial class AggregateMathboardModel : MathboardModel, IAggregateMathboardModel, IMathboardModel, IWhiteboardModel, ICloneable, IDisposable, ICalculatableContainer, ICalculatableAggregate, ICalculatable, ICanContainSymbolicCalculation, IVerticalWhiteboardSections
{
    protected AggregateMathboardModel(IWhiteboardItemPositionComparer itemPositionComparer)
        : base(itemPositionComparer)
    {
    }

    public event Action<ICalculatable> RequestEvaluation;

    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    public event Action<ICalculatable> TreatAsUpToDate;

    public abstract string Rl { get; }

    public virtual void Commit()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation(this);
        }
    }

    public void SetStatusSynchronized()
    {
        throw new InvalidOperationException("Logic error: ");
    }

    public virtual void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        throw new NotImplementedException("SetSynchronizationStatus should be overridden if it needs to be used in this AggregateMathboardModel");
    }

    public virtual void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        throw new NotImplementedException("SetCalculatingStatus should be overridden if it needs to be used in this AggregateMathboardModel");
    }

    public virtual void SetResult(IResultNode result)
    {
        throw new NotImplementedException("SetResult should be overridden if it needs to be used in this AggregateMathboardModel");
    }

    public virtual void SetError(EngineError error)
    {
        throw new NotImplementedException("SetError should be overridden if it needs to be used in this AggregateMathboardModel");
    }

    public virtual void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
        throw new NotImplementedException("SetIdentifierLabels should be overridden if it needs to be used in this AggregateMathboardModel");
    }

    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
    {
    }

    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
    {
        throw new NotImplementedException("SetRedefinitionWarnings should be overridden if it needs to be used in this AggregateMathboardModel");
    }

    public event Action SectionsMaybeChanged;

    public abstract int BlockSectionCount { get; }

    public abstract IEnumerable<IWhiteboardItem> GetBlockSectionItems(uint sectionIdx);

    public abstract string GetBlockSectionName(uint sectionIdx);

    public abstract double GetBlockSectionMinHeightRatio(uint sectionIdx);

    protected virtual void FireRequestEvaluation()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation(this);
        }
    }

    protected virtual IResultNode FireGetValueOfIdentifierBefore(string identifier)
    {
        if (GetValueOfIdentifierBefore != null)
        {
            return GetValueOfIdentifierBefore(this, identifier);
        }
        return null;
    }

    protected virtual void FireTreatAsUpToDate()
    {
        if (TreatAsUpToDate != null)
        {
            TreatAsUpToDate(this);
        }
    }

    protected virtual void FireSectionsMaybeChanged()
    {
        if (SectionsMaybeChanged != null)
        {
            SectionsMaybeChanged();
        }
    }

    public bool ContainsSymbolicEvaluation => CalculatableChildren.Any((ICalculatable child) => child is ICanContainSymbolicCalculation canContainSymbolicCalculation && canContainSymbolicCalculation.ContainsSymbolicEvaluation);

    // Explicit interface satisfaction for Quire prototype members on ICalculatable not present in Mathcad.
    void ICalculatable.SetResult(CalculationResult result) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    SynchronizationStatus ICalculatable.SynchronizationStatus => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    event EventHandler ICalculatable.RlChanged { add { } remove { } }
}
