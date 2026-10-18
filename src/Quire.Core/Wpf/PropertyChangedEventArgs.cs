#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class PropertyChangedEventArgs<TProperty>
{
    [MirrorVerified("PtcWpfPropertyChangedEventArgs1MechanicalDifferentialTests")]
    public TProperty OldValue { get; private set; }

    [MirrorVerified("PtcWpfPropertyChangedEventArgs1MechanicalDifferentialTests")]
    public TProperty NewValue { get; private set; }
}
