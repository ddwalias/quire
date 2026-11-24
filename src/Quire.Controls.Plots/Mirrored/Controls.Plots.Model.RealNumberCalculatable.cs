#nullable disable
#pragma warning disable

using Quire.Calculation.Core;

namespace Quire.Controls.Plots.Model;

public partial class RealNumberCalculatable
{
    private SynchronizationStatus _synchronizationStatus;

    public SynchronizationStatus SynchronizationStatus => _synchronizationStatus;

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        if (syncStatus != _synchronizationStatus)
        {
            _synchronizationStatus = syncStatus;
            OnPropertyChanged("SynchronizationStatus");
        }
    }
}
