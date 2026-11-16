#nullable disable
using System.Linq;
using Quire.Calculation.Core.Support;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class CollectionWithSyncStatus
{
    [MirrorVerified("PtcControlsIOCollectionWithSyncStatusMechanicalDifferentialTests")]
    public event Action<ICalcWithSyncStatus> Synchronized;

    private readonly List<ICalcWithSyncStatus> _items = new List<ICalcWithSyncStatus>();

    public CollectionWithSyncStatus() { }

    public bool Add(ICalcWithSyncStatus item)
    {
        if (_items.Contains(item))
        {
            return false;
        }
        _items.Add(item);
        item.Synchronized += OnShadowSynchronized;
        return true;
    }

    public global::Quire.Calculation.Core.SynchronizationStatus SynchronizationStatus
    {
        get
        {
            // Mathcad assigns Synchronized=3 and Pending=2 in this assembly's IL.
            ICalcWithSyncStatus item = _items.FirstOrDefault(item => item.SynchronizationStatus != (global::Quire.Calculation.Core.SynchronizationStatus)3);
            if (item == null)
            {
                return (global::Quire.Calculation.Core.SynchronizationStatus)3;
            }
            global::Quire.DebugTools.DevAssert(item.SynchronizationStatus == (global::Quire.Calculation.Core.SynchronizationStatus)0, null);
            return item.SynchronizationStatus;
        }
    }

    private void OnShadowSynchronized(ICalcWithSyncStatus shadow)
    {
        shadow.Synchronized -= OnShadowSynchronized;
        // Early returns preserve Mathcad's brtrue branch shape for this callback.
        if (Synchronized == null)
        {
            return;
        }
        if (SynchronizationStatus == (global::Quire.Calculation.Core.SynchronizationStatus)3)
        {
            Synchronized(this);
        }
    }
}
