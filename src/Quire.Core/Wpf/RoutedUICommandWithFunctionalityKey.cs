#nullable disable
using Quire.FunctionalitiesLimitation;
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class RoutedUICommandWithFunctionalityKey
{
    [MirrorVerified("PtcWpfRoutedUICommandWithFunctionalityKeyMechanicalDifferentialTests")]
    public RoutedUICommandWithFunctionalityKey() { }

    [MirrorVerified("PtcWpfRoutedUICommandWithFunctionalityKeyMechanicalDifferentialTests")]
    public EnumFunctionalityKey FunctionalityKey { get; protected set; }

    [MirrorVerified("Release IL")]
    public RoutedUICommandWithFunctionalityKey(string text, string name, Type ownerType, EnumFunctionalityKey key) : base(text, name, ownerType)
    {
        FunctionalityKey = key;
    }
}
