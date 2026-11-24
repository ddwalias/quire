#nullable disable
#pragma warning disable

using System;
using Quire.Calculation.Core;

namespace Quire.Controls.Plots.Model;

internal abstract partial class PlotCalculatable
{
    private SynchronizationStatus _synchronizationStatus = SynchronizationStatus.Synchronized;

    public SynchronizationStatus SynchronizationStatus
    {
        get
        {
            return _synchronizationStatus;
        }
        private set
        {
            if (value != _synchronizationStatus)
            {
                _synchronizationStatus = value;
                FirePropertyChanged("SynchronizationStatus");
            }
        }
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
}
