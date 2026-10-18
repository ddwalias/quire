#nullable disable
using System.Collections.Specialized;
using Quire.Mirror;

namespace Quire.Controls.IO;

public partial class ShadowCalculationNotifier
{
    [MirrorVerified("PtcControlsIOShadowCalculationNotifierMechanicalDifferentialTests")]
    public ShadowCalculationNotifier() { }

    [MirrorVerified("PtcControlsIOShadowCalculationNotifierMechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler CollectionChanged;
}
