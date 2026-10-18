#nullable disable
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class RoutedUICommandWithFunctionalityParameterKey
{
    [MirrorVerified("PtcWpfRoutedUICommandWithFunctionalityParameterKeyMechanicalDifferentialTests")]
    public RoutedUICommandWithFunctionalityParameterKey() { }

    [MirrorVerified("PtcWpfRoutedUICommandWithFunctionalityParameterKeyMechanicalDifferentialTests")]
    protected Dictionary<object, EnumFunctionalityKey> PrimaryParameterToFunctionalityKeyDictionary { get; private set; }
}
