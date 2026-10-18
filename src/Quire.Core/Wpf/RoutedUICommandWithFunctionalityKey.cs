#nullable disable
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class RoutedUICommandWithFunctionalityKey
{
    [MirrorVerified("PtcWpfRoutedUICommandWithFunctionalityKeyMechanicalDifferentialTests")]
    public RoutedUICommandWithFunctionalityKey() { }

    [MirrorVerified("PtcWpfRoutedUICommandWithFunctionalityKeyMechanicalDifferentialTests")]
    public EnumFunctionalityKey FunctionalityKey { get; protected set; }
}
