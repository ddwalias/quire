using System;
using System.ComponentModel;
using System.Windows;
using Quire.Calculation.Core;

using System.Linq;
using Quire.Controls.Core;
using System.Collections.Generic;
namespace Quire.Controls.ExcelComponent.Model;

internal abstract partial class OleBaseComponentCalculatableWrapper
{
    protected Action EquationChangedHandler { get; set; }

    public string LhsExpression { get; protected set; }

    public string RhsExpression { get; protected set; }

    public EngineError Error
    {
        get
        {
            return _error;
        }
        private set
        {
            WrappedCalculatable.SetError(value);
            if (_error == value)
            {
                return;
            }
            _error = value;
            if (ErrorChanged != null)
            {
                ErrorChanged(this, value);
            }
        }
    }

    public int ErrorCount
    {
        get
        {
            return Error != null ? 1 : 0;
        }
    }

    protected void ClearAllErrors()
    {
        Error = null;
    }

    public bool ContainsError(EngineError error)
    {
        return error.ErrorSource == this;
    }

    private void WrappedCalculatablePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        FirePropertyChanged(e);
    }

    private void OnEquationChanged(object sender, RoutedEventArgs args)
    {
        if (EquationChangedHandler != null)
        {
            EquationChangedHandler();
        }
    }

    public abstract Pair<bool, bool> IsFragment { get; }

    public abstract bool IsOutputEquation { get; }

    public abstract bool IsInputEquation { get; }

    public abstract bool IsInvalidEquation { get; }

    public abstract bool IsUnknownEquation { get; }

    protected abstract void ClearEquationType();

    private bool _isConnectedToTheEngine;

    private EngineError _error;

    public OleWrapperCustomErrorType CustomErrorType
    {
        get
        {
            if (Error != null)
            {
                return GetCustomErrorType(Error.ErrorCode);
            }
            return OleWrapperCustomErrorType.None;
        }
    }

    public bool IsValidEquation
    {
        get
        {
            if (!IsInputEquation)
            {
                return IsOutputEquation;
            }
            return true;
        }
    }

    public bool IsConnectedToTheEngine
    {
        get
        {
            return _isConnectedToTheEngine;
        }
        set
        {
            _isConnectedToTheEngine = value;
            if (_isConnectedToTheEngine)
            {
                return;
            }
            if (CustomErrorType == OleWrapperCustomErrorType.None || CustomErrorType == OleWrapperCustomErrorType.Calculation)
            {
                ClearAllErrors();
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual EngineResult EngineResult
    {
        get
        {
            if (!(WrappedCalculatable is ICalculatableResultOwner calculatableResultOwner))
            {
                return default;
            }
            return calculatableResultOwner.EngineResult;
        }
        set
        {
            ICalculatableResultOwner calculatableResultOwner = WrappedCalculatable as ICalculatableResultOwner;
            if (DebugTools.AssertNotNull(calculatableResultOwner, null).Passed)
            {
                calculatableResultOwner.EngineResult = value;
                OleWrapperCustomErrorType customErrorType = CustomErrorType;
                OleWrapperCustomErrorType oleWrapperCustomErrorType = value.Errors.IsNullOrEmpty() ? OleWrapperCustomErrorType.None : GetCustomErrorType(value.Errors[0].ErrorCode);
                if (customErrorType != OleWrapperCustomErrorType.None)
                {
                    DebugTools.DevAssert(oleWrapperCustomErrorType == OleWrapperCustomErrorType.None || oleWrapperCustomErrorType == customErrorType, null);
                }
                else if (oleWrapperCustomErrorType != OleWrapperCustomErrorType.None)
                {
                    DebugTools.DevAssert(Error == null, null);
                }
                Error = value.Errors.IsNullOrEmpty() ? null : value.Errors[0];
            }
        }
    }

    public virtual EngineResult EngineResultForSerialization
    {
        get
        {
            if (!(WrappedCalculatable is IResultOwnerWithSerializableEngineResult resultOwnerWithSerializableEngineResult))
            {
                return EngineResult;
            }
            return resultOwnerWithSerializableEngineResult.EngineResultForSerialization;
        }
    }

    public bool InterceptAndUpdateResult
    {
        get
        {
            if (!(WrappedCalculatable is ICalculatableResultOwner calculatableResultOwner))
            {
                return false;
            }
            return calculatableResultOwner.InterceptAndUpdateResult;
        }
    }

    public bool UseICalculatableToRestoreResult
    {
        get
        {
            if (!(WrappedCalculatable is ICalculatableResultOwner calculatableResultOwner))
            {
                return false;
            }
            return calculatableResultOwner.UseICalculatableToRestoreResult;
        }
    }

    public bool IsCommitted => ((ICalculatableInfo)WrappedCalculatable).IsCommitted;

    public SynchronizationStatus SynchronizationStatus => ((ICalculatableInfo)WrappedCalculatable).SynchronizationStatus;

    public CalculatingStatus CalculatingStatus => ((ICalculatableInfo)WrappedCalculatable).CalculatingStatus;

    public string ErrorSExpression => ((ICalculatableInfo)WrappedCalculatable).ErrorSExpression;

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

    public EngineError CurrentError
    {
        get
        {
            if (WrappedCalculatable is ICalculationErrorOwner calculationErrorOwner)
            {
                return calculationErrorOwner.CurrentError;
            }
            return null;
        }
    }

    public bool CanHideFromCalculationContext
    {
        get
        {
            ICalculationHideableChild calculationHideableChild = WrappedCalculatable as ICalculationHideableChild;
            if (DebugTools.AssertNotNull(calculationHideableChild, null).Passed)
            {
                return calculationHideableChild.CanHideFromCalculationContext;
            }
            return false;
        }
    }

    public bool HideFromCalculationContext
    {
        get
        {
            ICalculationHideableChild calculationHideableChild = WrappedCalculatable as ICalculationHideableChild;
            if (DebugTools.AssertNotNull(calculationHideableChild, null).Passed)
            {
                return calculationHideableChild.HideFromCalculationContext;
            }
            return false;
        }
        set
        {
            ICalculationHideableChild calculationHideableChild = WrappedCalculatable as ICalculationHideableChild;
            if (DebugTools.AssertNotNull(calculationHideableChild, null).Passed)
            {
                calculationHideableChild.HideFromCalculationContext = value;
            }
        }
    }

    public override ICalculatable WrappedCalculatable
    {
        get
        {
            return base.WrappedCalculatable;
        }
        protected set
        {
            base.WrappedCalculatable = value;
            DebugTools.DevAssert(false, "This should never be called");
        }
    }

    protected bool IsSupportedWrappedCalculatable
    {
        get
        {
            if (WrappedCalculatable != null)
            {
                if (WrappedCalculatable is ICalculatableAggregate)
                {
                    return ((ICalculatableAggregate)WrappedCalculatable).CalculatableChildren.Count() == 0;
                }
                return true;
            }
            return false;
        }
    }

    public event Action<IOleBaseComponentCalculatableWrapper, EngineError> ErrorChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    protected OleBaseComponentCalculatableWrapper(ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
        EquationChangedHandler = ClearEquationType;
        HookupCalculatableEvents();
    }

    protected virtual OleWrapperCustomErrorType GetCustomErrorType(string errorCode)
    {
        switch (errorCode)
        {
            case "WronglyFormattedInput.Error.Key":
            case "WronglyFormattedOutput.Error.Key":
                return OleWrapperCustomErrorType.Formatting;
            case "WronglyPlacedInput.Error.Key":
            case "WronglyPlacedOutput.Error.Key":
                return OleWrapperCustomErrorType.Placement;
            default:
                return OleWrapperCustomErrorType.None;
        }
    }

    protected void SetCustomError(EngineError error)
    {
        DebugTools.DevAssert(error == null || GetCustomErrorType(error.ErrorCode) != OleWrapperCustomErrorType.None, null);
        if (error == null && Error != null && CustomErrorType == OleWrapperCustomErrorType.None)
        {
            return;
        }
        if (Error != null && error != null && Error.ErrorCode == error.ErrorCode)
        {
            return;
        }
        Error = error;
    }

    public void SetCustomError(string errorMessage, EngineError originalError = null)
    {
        SetCustomError(errorMessage.IsNullOrEmpty() ? null : new EngineError
        {
            Message = errorMessage,
            OriginalError = originalError,
            ErrorSource = this
        });
    }

    public void RestoreResult()
    {
        if (WrappedCalculatable is ICalculatableResultOwner calculatableResultOwner)
        {
            calculatableResultOwner.RestoreResult();
        }
    }

    public void ShowError(EngineError error)
    {
        if (WrappedCalculatable is ICalculationErrorOwner calculationErrorOwner)
        {
            calculationErrorOwner.ShowError(error);
        }
    }

    public override void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
        Action equationChangedHandler = EquationChangedHandler;
        try
        {
            EquationChangedHandler = null;
            base.SetIdentifierLabels(idLabelMap);
        }
        finally
        {
            EquationChangedHandler = equationChangedHandler;
        }
    }

    public override void SetError(EngineError error)
    {
        if (error == null && CustomErrorType == OleWrapperCustomErrorType.Calculation)
        {
            return;
        }
        OleWrapperCustomErrorType customErrorType = CustomErrorType;
        DebugTools.DevAssert((error == null ? OleWrapperCustomErrorType.None : GetCustomErrorType(error.ErrorCode)) == OleWrapperCustomErrorType.None && (customErrorType == OleWrapperCustomErrorType.None || customErrorType == OleWrapperCustomErrorType.Calculation), null);
        Error = error;
    }

    public override void Dispose()
    {
        UnhookCalculatableEvents();
        base.Dispose();
    }

    private void FirePropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, eventArgs);
        }
    }

    private void HookupCalculatableEvents()
    {
        if (WrappedCalculatable is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += WrappedCalculatablePropertyChanged;
        }
        if (WrappedCalculatable is IEquationControl equationControl)
        {
            equationControl.Changed += OnEquationChanged;
        }
    }

    private void UnhookCalculatableEvents()
    {
        if (WrappedCalculatable is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged -= WrappedCalculatablePropertyChanged;
        }
        if (WrappedCalculatable is IEquationControl equationControl)
        {
            equationControl.Changed -= OnEquationChanged;
        }
    }
}
