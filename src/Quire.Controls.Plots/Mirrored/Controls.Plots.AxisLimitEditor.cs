#nullable disable
#pragma warning disable

using Quire.Calculation.Core;

namespace Quire.Controls.Plots;

public partial class AxisLimitEditor
{
    public SynchronizationStatus SynchronizationStatus => _eq.SynchronizationStatus;

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        _eq.SetSynchronizationStatus(syncStatus);
    }
}
