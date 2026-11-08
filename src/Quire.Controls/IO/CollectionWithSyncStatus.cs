#nullable disable
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
}
