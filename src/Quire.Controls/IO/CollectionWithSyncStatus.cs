#nullable disable
using Quire.Calculation.Core.Support;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class CollectionWithSyncStatus
{
    [MirrorVerified("PtcControlsIOCollectionWithSyncStatusMechanicalDifferentialTests")]
    public event Action<ICalcWithSyncStatus> Synchronized;
}
