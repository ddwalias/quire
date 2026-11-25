#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Quire;
using Quire.Calculation.Core;

namespace Quire.Controls.Plots.Model;

public partial class RealNumberCalculatable
{
    public static partial class PropertyNames
    {
        public const string Value = "Value";
        public const string ErrorCount = "ErrorCount";
        public const string ErrorCode = "ErrorCode";
        public const string ErrorText = "ErrorText";
        public const string Error = "Error";
    }

    private IEquationProxy _equationProxy;
    private double _value;
    private SynchronizationStatus _synchronizationStatus;
    private CalculatingStatus _calculatingStatus = CalculatingStatus.Idle;
    private EngineError _engineError;
    private readonly List<string> _customErrors = new List<string>();
    private EngineError _error;
    private bool _hideFromCalculationContext;
    private bool _overrideHideFromCalculation = true;

    public IEquationProxy EquationProxy
    {
        get => _equationProxy;
        set
        {
            if (_equationProxy != null)
            {
                _equationProxy.RequestEvaluation -= ProxyRequestEvaluation;
            }
            _equationProxy = value;
            if (_equationProxy != null)
            {
                _equationProxy.RequestEvaluation += ProxyRequestEvaluation;
            }
        }
    }

    private void ProxyRequestEvaluation(IEquationProxy obj)
    {
        Commit();
    }

    public double Value
    {
        get => _value;
        private set
        {
            _value = value;
            OnPropertyChanged("Value");
        }
    }

    public event Action<ICalculatable> RequestEvaluation;
    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;
    public event Action<ICalculatable> TreatAsUpToDate;

    public string Rl
    {
        get
        {
            if (EquationProxy == null)
            {
                return "-PLACEHOLDER:0x00000001";
            }
            return EquationProxy.Rl;
        }
    }

    public void Commit()
    {
        if (EquationProxy != null)
        {
            if (!IsCommitted)
            {
                EquationProxy.IsCommitted = true;
            }
            if (EquationProxy.IsCleared)
            {
                OverrideHideFromCalculation = true;
            }
        }
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

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        if (syncStatus != _synchronizationStatus)
        {
            _synchronizationStatus = syncStatus;
            OnPropertyChanged("SynchronizationStatus");
        }
    }

    public void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        if (computingStatus != _calculatingStatus)
        {
            _calculatingStatus = computingStatus;
            OnPropertyChanged("CalculatingStatus");
        }
    }

    public void SetResult(IResultNode result)
    {
        ClearCustomErrors("RESULT_NOT_REAL_NUMBER", "Ptc.Plots.MARKERS_HAS_UNITS");
        if (result == null || result is IEmptyResultNode)
        {
            return;
        }
        IRealResultNode realResultNode = result as IRealResultNode;
        if (realResultNode != null && !double.IsNaN(realResultNode.Value))
        {
            Value = realResultNode.Value;
            return;
        }
        if (result is IUnitedValueResultNode)
        {
            SetCustomError("Ptc.Plots.MARKERS_HAS_UNITS");
            return;
        }
        SetCustomError("RESULT_NOT_REAL_NUMBER");
    }

    private EngineError FirstCustomError
    {
        get
        {
            if (_customErrors.Count <= 0)
            {
                return null;
            }
            return new EngineError
            {
                Message = _customErrors[0]
            };
        }
    }

    private void UpdateError()
    {
        Error = _engineError ?? FirstCustomError;
    }

    public void SetError(EngineError error)
    {
        _engineError = error;
        UpdateError();
    }

    private void ClearEngineError()
    {
        _engineError = null;
        UpdateError();
    }

    public void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
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

    public void SetCustomError(string errorCode)
    {
        if (!_customErrors.Contains(errorCode))
        {
            _customErrors.Add(errorCode);
            UpdateError();
        }
    }

    public void ClearCustomErrors(string errorCode, params string[] errorCodes)
    {
        bool flag = _customErrors.Remove(errorCode);
        foreach (string item in errorCodes)
        {
            flag |= _customErrors.Remove(item);
        }
        if (flag)
        {
            UpdateError();
        }
    }

    public EngineResult EngineResult { get; set; }

    public bool InterceptAndUpdateResult => true;

    public bool UseICalculatableToRestoreResult => true;

    public void RestoreResult()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(params string[] propertyNames)
    {
        if (PropertyChanged != null)
        {
            foreach (string propertyName in propertyNames)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public SynchronizationStatus SynchronizationStatus => _synchronizationStatus;

    public CalculatingStatus CalculatingStatus => _calculatingStatus;

    public int ErrorCount => (Error != null) ? 1 : 0;

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

    public bool IsCommitted => EquationProxy.IsCommitted;

    public EngineError Error
    {
        get => _error;
        private set
        {
            if (value != _error)
            {
                _error = value;
                OnPropertyChanged("ErrorCount", "ErrorCode", "ErrorText", "Error");
            }
        }
    }

    public bool CanHideFromCalculationContext => true;

    public bool HideFromCalculationContext
    {
        get
        {
            if (!OverrideHideFromCalculation)
            {
                return _hideFromCalculationContext;
            }
            return true;
        }
        set
        {
            bool hideFromCalculationContext = HideFromCalculationContext;
            _hideFromCalculationContext = value;
            if (hideFromCalculationContext != HideFromCalculationContext)
            {
                OnPropertyChanged("HideFromCalculationContext");
            }
        }
    }

    public bool ContainsError(EngineError error)
    {
        return Equals(error.ErrorSource);
    }

    public event EventHandler<RequestShowErrorEventArgs> RequestShowError;

    public void ShowError(EngineError error)
    {
        if (RequestShowError != null)
        {
            RequestShowError(this, new RequestShowErrorEventArgs(error));
        }
    }

    public EngineError CurrentError => Error;

    public bool OverrideHideFromCalculation
    {
        get => _overrideHideFromCalculation;
        set
        {
            bool hideFromCalculationContext = HideFromCalculationContext;
            _overrideHideFromCalculation = value;
            if (hideFromCalculationContext != HideFromCalculationContext)
            {
                OnPropertyChanged("HideFromCalculationContext");
            }
            if (value)
            {
                ClearEngineError();
            }
        }
    }

    public RealNumberCalculatable()
    {
    }
}
