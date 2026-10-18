#nullable disable
using System.Collections.Specialized;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ObservableRangeCollection<T>
{
    [MirrorVerified("PtcWpfObservableRangeCollection1MechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler RangeChanging;

    [MirrorVerified("PtcWpfObservableRangeCollection1MechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler RangeChanged;
}
