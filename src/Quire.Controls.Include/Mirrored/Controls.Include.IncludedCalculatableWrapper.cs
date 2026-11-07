#nullable disable
#pragma warning disable
using System;
using System.ComponentModel;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;

namespace Quire.Controls.Include;

public partial class IncludedCalculatableWrapper
{
    private bool _isCommited;

    public event Delegate1<EngineError> SetErrorChanged;

    public event Action SynchronizationStatusChangedEvent;

    public event Action CalculatingStatusChangedEvent;

    public bool IsInternalCalc { get; set; }

    public ICalculatableInfo CalculatableInfo => WrappedCalculatable as ICalculatableInfo;

    public IncludedCalculatableWrapper(ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
        _isCommited = false;
    }

    public override void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        base.SetSynchronizationStatus(syncStatus);
        FireSynchronizationStatusChanged();
        FirePropertyChanged(SyncStatusChangedEventArgs.GetArgs());
    }

    public override void SetCalculatingStatus(CalculatingStatus calculatingStatus)
    {
        base.SetCalculatingStatus(calculatingStatus);
        FireCalculatingStatusChanged();
        FirePropertyChanged(CalculatingStatusChangedEventArgs.GetArgs());
    }

    public override void SetError(EngineError error)
    {
        base.SetError(error);
        FireSetErrorChanged(error);
        FirePropertyChanged(ErrorChangedEventArgs.GetArgs());
    }

    public bool ContainsError(EngineError error)
    {
        if (Equals(error.ErrorSource))
        {
            return true;
        }
        return false;
    }

    public void ShowError(EngineError error)
    {
        if (WrappedCalculatable is ICalculationErrorOwner calculationErrorOwner)
        {
            calculationErrorOwner.ShowError(error);
        }
    }

    public EngineError CurrentError => Error;

    public SynchronizationStatus SynchronizationStatus
    {
        get
        {
            if (WrappedCalculatable is ICalculatableInfo)
            {
                return (WrappedCalculatable as ICalculatableInfo).SynchronizationStatus;
            }
            // Release IL requires enum value 3 (Synchronized in Mathcad)
            return (SynchronizationStatus)3;
        }
    }

    public CalculatingStatus CalculatingStatus
    {
        get
        {
            if (WrappedCalculatable is ICalculatableInfo)
            {
                return (WrappedCalculatable as ICalculatableInfo).CalculatingStatus;
            }
            // Release IL requires enum value 2 (NotComputing in Mathcad)
            return (CalculatingStatus)2;
        }
    }

    public int ErrorCount
    {
        get
        {
            if (WrappedCalculatable is ICalculatableInfo)
            {
                return (WrappedCalculatable as ICalculatableInfo).ErrorCount;
            }
            return 0;
        }
    }

    public string ErrorSExpression
    {
        get
        {
            if (WrappedCalculatable is ICalculatableInfo)
            {
                return (WrappedCalculatable as ICalculatableInfo).ErrorSExpression;
            }
            return null;
        }
    }

    public string ErrorCode
    {
        get
        {
            if (WrappedCalculatable is ICalculatableInfo)
            {
                return (WrappedCalculatable as ICalculatableInfo).ErrorCode;
            }
            return null;
        }
    }

    public string ErrorText
    {
        get
        {
            if (WrappedCalculatable is ICalculatableInfo)
            {
                return (WrappedCalculatable as ICalculatableInfo).ErrorText;
            }
            return null;
        }
    }

    public bool IsCommitted
    {
        get
        {
            if (WrappedCalculatable is ICalculatableInfo)
            {
                return (WrappedCalculatable as ICalculatableInfo).IsCommitted;
            }
            return true;
        }
        set
        {
            _isCommited = value;
        }
    }

    public EngineError Error
    {
        get
        {
            if (WrappedCalculatable is ICalculatableInfo)
            {
                return (WrappedCalculatable as ICalculatableInfo).Error;
            }
            return null;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void FirePropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChangedEventHandler propertyChanged = PropertyChanged;
        if (propertyChanged != null)
        {
            propertyChanged(this, e);
        }
    }

    private void FireSetErrorChanged(EngineError error)
    {
        Delegate1<EngineError> setErrorChanged = SetErrorChanged;
        if (setErrorChanged != null)
        {
            setErrorChanged(error);
        }
    }

    private void FireSynchronizationStatusChanged()
    {
        Action synchronizationStatusChangedEvent = SynchronizationStatusChangedEvent;
        if (synchronizationStatusChangedEvent != null)
        {
            synchronizationStatusChangedEvent();
        }
    }

    private void FireCalculatingStatusChanged()
    {
        Action calculatingStatusChangedEvent = CalculatingStatusChangedEvent;
        if (calculatingStatusChangedEvent != null)
        {
            calculatingStatusChangedEvent();
        }
    }
}
