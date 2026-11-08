#nullable disable
using System.Collections.Specialized;
using Quire.Calculation.Core;
using Quire.Controls.Core.IORegions;
using Quire.Mirror;

namespace Quire.Controls.IO;

public partial class ShadowCalculationNotifier
{
    [MirrorVerified("PtcControlsIOShadowCalculationNotifierMechanicalDifferentialTests")]
    public ShadowCalculationNotifier() { }

    [MirrorVerified("PtcControlsIOShadowCalculationNotifierMechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public void Add(Pair<IIORegion, ICalculatable> item)
    {
        if (CollectionChanged != null)
        {
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            CollectionChanged(this, e);
        }
    }

    public void Remove(Pair<IIORegion, ICalculatable> item)
    {
        if (CollectionChanged != null)
        {
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            CollectionChanged(this, e);
        }
    }
}
