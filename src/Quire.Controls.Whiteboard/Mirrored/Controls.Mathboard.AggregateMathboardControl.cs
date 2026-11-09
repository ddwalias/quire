#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.Mathboard;

public abstract partial class AggregateMathboardControl : MathboardControl, IAggregateMathboardControl, IMathboardControl, IWhiteboardControl, IWhiteboardItemContainer, IHasRegionsWithConverterAnnotations, IMathFormatDeserializationProvider, IMaybeHasPointThatSnapsToPixel, IHasChildArrangeInfoProvider, IDisposable, ICalculatableContainer, ICalculationHideableChild, INotifyPropertyChanged, IWhiteboardItem, IHelpContext, ICalculationErrorOwner, IIORegionContainer, IIORegionBase, IHideFromCalculationButtonStatus, ICalculatableAggregate, ICalculatable, ICanContainSymbolicCalculation, ICalculatableInfo, ICalculatableResultOwner
{
    protected IAggregateMathboardModel AggregateModel => (IAggregateMathboardModel)_model;

    protected IAggregateMathboardViewModel AggregateViewModel => (IAggregateMathboardViewModel)ViewModel;

    protected AggregateMathboardControl(IMathboardViewModel viewModel, ResourceDictionary dict)
        : base(viewModel, dict)
    {
        BindProperties();
        HookupEvents();
        BindCommands();
    }

    protected override void ViewModelPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        base.ViewModelPropertyChangedHandler(sender, e);
        if (e.PropertyName == "HideFromCalculationContext")
        {
            FirePropertyChanged(e);
        }
    }

    public override bool HideFromCalculationContext
    {
        get => AggregateViewModel.HideFromCalculationContext;
        set => AggregateViewModel.HideFromCalculationContext = value;
    }

    private void BindProperties()
    {
        BindProperty("HideFromCalculationContext", AttachedProperties.CalcIsDisabledProperty, BindingMode.OneWay);
    }

    private void HookupEvents()
    {
        AggregateModel.RequestEvaluation += (ICalculatable calc) =>
        {
            FireRequestEvaluation();
        };
    }

    private void BindCommands()
    {
    }

    public event Action<ICalculatable> RequestEvaluation;

    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    public event Action<ICalculatable> TreatAsUpToDate;

    public string Rl => AggregateModel.Rl;

    public virtual void Commit()
    {
        MathboardViewModel.Commit();
        FireRequestEvaluation();
    }

    public virtual void SetStatusSynchronized()
    {
        FireTreatAsUpToDate();
    }

    public virtual void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
    }

    public virtual void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
    }

    public virtual void SetResult(IResultNode result)
    {
    }

    public virtual void SetError(EngineError error)
    {
    }

    public virtual void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
    }

    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
    {
    }

    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
    {
    }

    // IL requires constant 3 for Mathcad's SynchronizationStatus.Synchronized
    public virtual SynchronizationStatus SynchronizationStatus => (SynchronizationStatus)3;

    // IL requires constant 2 for Mathcad's CalculatingStatus.NotComputing
    public virtual CalculatingStatus CalculatingStatus => (CalculatingStatus)2;

    public virtual int ErrorCount => (Error != null) ? 1 : 0;

    public string ErrorSExpression => throw new NotImplementedException();

    public string ErrorCode
    {
        get
        {
            if (Error == null)
            {
                return string.Empty;
            }
            return Error.ErrorCode;
        }
    }

    public string ErrorText
    {
        get
        {
            if (Error == null)
            {
                return string.Empty;
            }
            return Error.ResourceString;
        }
    }

    public bool IsCommitted => true;

    public virtual EngineError Error => null;

    public virtual EngineResult EngineResult { get; set; }

    public bool InterceptAndUpdateResult => true;

    public bool UseICalculatableToRestoreResult => true;

    public void RestoreResult()
    {
        throw new NotImplementedException();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        constraint = base.MeasureOverride(constraint);
        return AggregateViewModel.MeasureOverrideHelper(constraint);
    }

    public IEnumerable<ISectionIndicatorInfo> SectionIndicators => AggregateViewModel.SectionIndicators;

    protected void FireRequestEvaluation()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation(this);
        }
    }

    protected IResultNode FireGetValueOfIdentifierBefore(string identifier)
    {
        if (GetValueOfIdentifierBefore != null)
        {
            return GetValueOfIdentifierBefore(this, identifier);
        }
        return null;
    }

    protected void FireTreatAsUpToDate()
    {
        if (TreatAsUpToDate != null)
        {
            TreatAsUpToDate(this);
        }
    }

    protected void DeserializeRegionsEpilog(IDeS11NProvider deS11NProvider, mathBoardType mathBoardType)
    {
        HideFromCalculationContext = mathBoardType.IsCalculationDisabled;
        DeserializationStrategy.DeserializeRegionsEpilog(deS11NProvider.XmlContentDocument);
    }

    public void SerializeRegionsEpilog(IS11NProvider s11NProvider, mathBoardType mathBoardType)
    {
        mathBoardType.IsCalculationDisabled = HideFromCalculationContext;
        SerializationStrategy.SerializeRegionsEpilog(s11NProvider.XmlContentDocument);
    }

    public bool ContainsSymbolicEvaluation => AggregateModel.ContainsSymbolicEvaluation;

}
