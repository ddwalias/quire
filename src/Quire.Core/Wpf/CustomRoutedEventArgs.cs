#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class CustomRoutedEventArgs<T>
{
    [MirrorVerified("PtcWpfCustomRoutedEventArgs1MechanicalDifferentialTests")]
    public readonly T Parameter;

    [MirrorVerified("PtcWpfCustomRoutedEventArgs1MechanicalDifferentialTests")]
    public CustomRoutedEventArgs(RoutedEvent routedEvent, T param) : base(routedEvent)
    {
        Parameter = param;
    }
}
