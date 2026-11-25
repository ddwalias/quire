#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Quire;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Undo;

namespace Quire.Controls.Plots.Model;

internal abstract partial class PlotCalculatable : IPlotCalculatable, ICalculatableInfo, INotifyPropertyChanged, ICalculationHideableChild, ICalculatableResultOwner, ICalculatable, ICalculationErrorOwner, IHasNumberOfPoints, ICalcWithCache
{
    public partial class PropertyNames
    {
        public const string CalculatingStatus = "CalculatingStatus";
        public const string Error = "Error";
        public const string HideFromCalculationContext = "HideFromCalculationContext";
        public const string SynchronizationStatus = "SynchronizationStatus";
        public const string IsNumOfPointsMutable = "IsNumOfPointsMutable";
        public const string NumberOfPoints = "NumberOfPoints";

        protected PropertyNames()
        {
        }
    }

    private SynchronizationStatus _synchronizationStatus = SynchronizationStatus.Synchronized;
    private CalculatingStatus _calculatingStatus = CalculatingStatus.Idle;
    private EngineError _error;
    private bool _hideFromCalculationContext;
    private readonly NodeHandleMapper _nodeHandleMapper = new NodeHandleMapper();

    public event PropertyChangedEventHandler PropertyChanged;
    public event Action<ICalculatable> RequestEvaluation;

    protected void FireRequestEvaluation()
    {
        Action<ICalculatable> action = RequestEvaluation;
        if (action != null)
        {
            BeforeRequestEvaluation();
            action(this);
        }
    }

    protected virtual void BeforeRequestEvaluation()
    {
    }

    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;
    public event Action<ICalculatable> TreatAsUpToDate;

    public abstract string Rl { get; }
    public abstract void Commit();
    public abstract void SetResult(IResultNode result);

    public void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
        Dictionary<ICalculatable, IDictionary<ResolvedId, string[]>> dictionary = new Dictionary<ICalculatable, IDictionary<ResolvedId, string[]>>();
        foreach (KeyValuePair<ResolvedId, string[]> item in idLabelMap)
        {
            uint tag = (uint)item.Key.Tag;
            ICalculatable calculatable = FindChildWithMatchingId(tag);
            if (calculatable != null)
            {
                uint mappedNodeNandle = _nodeHandleMapper.GetMappedNodeNandle(tag);
                ResolvedId key = new ResolvedId
                {
                    Calculatable = item.Key.Calculatable,
                    Name = item.Key.Name,
                    Tag = (int)mappedNodeNandle
                };
                IDictionary<ResolvedId, string[]> dictionary2;
                if (dictionary.ContainsKey(calculatable))
                {
                    dictionary2 = dictionary[calculatable];
                }
                else
                {
                    dictionary2 = new Dictionary<ResolvedId, string[]>();
                    dictionary.Add(calculatable, dictionary2);
                }
                dictionary2.Add(key, item.Value);
            }
        }
        foreach (KeyValuePair<ICalculatable, IDictionary<ResolvedId, string[]>> item2 in dictionary)
        {
            item2.Key.SetIdentifierLabels(item2.Value);
        }
    }

    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
    {
    }

    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
    {
        if (warnings.IsNotEmpty())
        {
            throw new NotImplementedException();
        }
    }

    public EngineError CurrentError => Error;

    public SynchronizationStatus SynchronizationStatus
    {
        get => _synchronizationStatus;
        private set
        {
            if (value != _synchronizationStatus)
            {
                _synchronizationStatus = value;
                FirePropertyChanged("SynchronizationStatus");
            }
        }
    }

    public CalculatingStatus CalculatingStatus
    {
        get => _calculatingStatus;
        private set
        {
            if (_calculatingStatus != value)
            {
                _calculatingStatus = value;
                FirePropertyChanged("CalculatingStatus");
            }
        }
    }

    public virtual EngineResult EngineResult { get; set; }

    public EngineError Error
    {
        get => _error;
        protected set
        {
            if (_error != value)
            {
                _error = value;
                FirePropertyChanged("Error");
            }
        }
    }

    public IResultNode Result => EngineResult.ResultNode;

    public IList<EngineRedefInfo> EngineWarnings => null;

    public string ErrorCode
    {
        get
        {
            if (Error != null)
            {
                return Error.ErrorCode;
            }
            return null;
        }
    }

    public string ErrorText
    {
        get
        {
            if (Error != null)
            {
                return Error.ResourceString;
            }
            return null;
        }
    }

    public string ErrorSExpression
    {
        get
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return string.Empty;
        }
    }

    public bool CanHideFromCalculationContext => true;

    public bool HideFromCalculationContext
    {
        get => _hideFromCalculationContext;
        set
        {
            if (_hideFromCalculationContext != value)
            {
                DelegateFunction0<bool> doMethod = () => SetHideFromCalculationContext(value);
                Delegate0 undoMethod = () =>
                {
                    SetHideFromCalculationContext(!value);
                };
                CommandBuffer.Execute(new UndoableCommand("Change HideFromCalculationContext", doMethod, undoMethod));
            }
        }
    }

    public bool InterceptAndUpdateResult => true;

    protected ICalculatable EquationWithError { get; set; }

    public bool UseICalculatableToRestoreResult => true;

    public int ErrorCount => (Error != null) ? 1 : 0;

    public abstract bool IsCommitted { get; }

    private int _numberOfPoints = -1;
    private bool _isNumOfPointsMutable;

    public int NumberOfPoints
    {
        get => _numberOfPoints;
        set
        {
            if (_numberOfPoints != value)
            {
                _numberOfPoints = value;
                FirePropertyChanged("NumberOfPoints");
            }
        }
    }

    public bool IsNumOfPointsMutable
    {
        get => _isNumOfPointsMutable;
        protected set
        {
            if (_isNumOfPointsMutable != value)
            {
                _isNumOfPointsMutable = value;
                FirePropertyChanged("IsNumOfPointsMutable");
            }
        }
    }

    public event EventHandler<EngineErrorEventArgs> PlotCalculatableError;

    protected void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        OnChanged();
    }

    public event Action Changed;

    private void OnChanged()
    {
        if (Changed != null)
        {
            Changed();
        }
    }

    public bool ContainsError(EngineError error)
    {
        return Equals(error.ErrorSource);
    }

    private bool SetHideFromCalculationContext(bool value)
    {
        if (_hideFromCalculationContext != value)
        {
            _hideFromCalculationContext = value;
            FirePropertyChanged("HideFromCalculationContext");
            return true;
        }
        return false;
    }

    public void RestoreResult()
    {
        DebugTools.DevAssert(false, "RestoreResult is not implemented on PlotCalculatable.");
    }

    public void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        CalculatingStatus = computingStatus;
    }

    public virtual void SetError(EngineError error)
    {
        if (error == null)
        {
            CleanErrors();
            return;
        }
        Error = error;
        ICalculatable calculatable = FindChildWithMatchingId(error.NodeHandle);
        if (calculatable != null)
        {
            PreprocessEquationSpecificError();
            Error.NodeHandle = _nodeHandleMapper.GetMappedNodeNandle(error.NodeHandle);
            EquationWithError = calculatable;
            calculatable.SetError(error);
            OnEquationSpecificError(calculatable);
        }
        else
        {
            OnPlotCalculatableError(new EngineErrorEventArgs(error));
        }
    }

    protected virtual void OnTraceEquationSExpressionChanged()
    {
        CleanErrors();
    }

    protected void CleanErrors()
    {
        ClearEquationError(EquationWithError);
        EquationWithError = null;
        Error = null;
        OnPlotCalculatableError(new EngineErrorEventArgs(null));
    }

    protected void ClearEquationError(ICalculatable equationControl)
    {
        if (equationControl != null && Error != null && Error.IsEquivalentTo(((ICalculatableInfo)equationControl).Error))
        {
            equationControl.SetError(null);
        }
    }

    protected virtual void PreprocessEquationSpecificError()
    {
    }

    protected virtual void OnEquationSpecificError(ICalculatable equation)
    {
    }

    protected ICalculatable FindChildWithMatchingId(uint handle)
    {
        return _nodeHandleMapper.GetMappedEquation(handle);
    }

    protected virtual void OnPlotCalculatableError(EngineErrorEventArgs args)
    {
        if (PlotCalculatableError != null)
        {
            PlotCalculatableError(this, args);
        }
        if (args.EngineError != null && IsApplicationCustomError(args.EngineError))
        {
            EngineResult engineResult = new EngineResult(null, new EngineErrorCollection { args.EngineError }, EngineResult.SynchronizationStatus);
            EngineResult = engineResult;
        }
    }

    private static bool IsApplicationCustomError(EngineError error)
    {
        if (error != null && error.Argument == 0 && error.NodeHandle == 0 && error.OriginalError == null)
        {
            return error.ErrorSource == null;
        }
        return false;
    }

    public void SetStatusSynchronized()
    {
        Action<ICalculatable> treatAsUpToDate = TreatAsUpToDate;
        if (treatAsUpToDate != null)
        {
            treatAsUpToDate(this);
        }
    }

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        SynchronizationStatus = syncStatus;
    }

    public void ShowError(EngineError error)
    {
        if (EquationWithError is ICalculationErrorOwner calculationErrorOwner)
        {
            calculationErrorOwner.ShowError(error);
        }
    }

    protected void ClearNodeHandleMapper()
    {
        _nodeHandleMapper.Clear();
    }

    protected string GetReplacedRl(IEquationControl equationControl)
    {
        return _nodeHandleMapper.GetReplacedRl(equationControl);
    }

    public string GetReplacedRl(IEquationControl equationControl, string rl)
    {
        return _nodeHandleMapper.GetReplacedRl(equationControl, rl);
    }

    protected void AttachOnTraceEquationSExpressionChanged(IPlotEquation plotEquation)
    {
        if (DebugTools.DevAssert(plotEquation != null, null).Passed)
        {
            plotEquation.SExpressionValueChanged += OnTraceEquationSExpressionChanged;
        }
    }

    protected void DetachOnTraceEquationSExpressionChanged(IPlotEquation plotEquation)
    {
        if (DebugTools.DevAssert(plotEquation != null, null).Passed)
        {
            plotEquation.SExpressionValueChanged -= OnTraceEquationSExpressionChanged;
        }
    }

    protected PlotCalculatable()
    {
    }
}
