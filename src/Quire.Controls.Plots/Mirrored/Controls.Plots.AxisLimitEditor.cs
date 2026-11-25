#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Quire.Calculation.Core;
using Quire.Core;

namespace Quire.Controls.Plots;

public partial class AxisLimitEditor
{
    public string Rl => string.Format("(eval {0})", _eq.Rl);

    public bool CanHideFromCalculationContext => true;

    public EngineError CurrentError => Error;

    public EngineResult EngineResult { get; set; }

    public bool InterceptAndUpdateResult => true;

    public bool UseICalculatableToRestoreResult => true;

    public SynchronizationStatus SynchronizationStatus => _eq.SynchronizationStatus;

    public CalculatingStatus CalculatingStatus => _eq.CalculatingStatus;

    public int ErrorCount => _eq.ErrorCount;

    public string ErrorSExpression => _eq.ErrorSExpression;

    public string ErrorCode => _eq.ErrorCode;

    public string ErrorText => _eq.ErrorText;

    public bool IsCommitted
    {
        get
        {
            if (_eq.IsCommitted)
            {
                if (!IsDefaultValue && EngineResult.IsEmpty())
                {
                    return SynchronizationStatus == SynchronizationStatus.Pending;
                }
                return true;
            }
            return false;
        }
    }

    public EngineError Error => _eq.Error;

    public event Action ValueChanged;

    public event Action HasErrorsChanged;

    public event Action<ICalculatable> RequestEvaluation;

    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    public event Action<ICalculatable> TreatAsUpToDate;

    public event PropertyChangedEventHandler PropertyChanged;

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

    public void SetStatusSynchronized()
    {
        if (TreatAsUpToDate != null)
        {
            TreatAsUpToDate(this);
        }
    }

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        _eq.SetSynchronizationStatus(syncStatus);
    }

    public void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        _eq.SetCalculatingStatus(computingStatus);
    }

    private void FireValueChanged()
    {
        if (ValueChanged != null)
        {
            ValueChanged();
        }
    }

    private void FireHasErrorsChanged()
    {
        if (HasErrorsChanged != null)
        {
            HasErrorsChanged();
        }
    }

    private void FirePropertyChanged(PropertyChangedEventArgs e)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, e);
        }
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
        ICalculationErrorOwner calculationErrorOwner = _eq;
        if (calculationErrorOwner != null)
        {
            calculationErrorOwner.ShowError(error);
        }
    }

    public void RestoreResult()
    {
        throw new NotImplementedException();
    }
}
