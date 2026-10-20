#nullable disable
using Quire.FunctionalitiesLimitation;
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class RoutedUICommandWithFunctionalityParameterKey
{
    [MirrorVerified("PtcWpfRoutedUICommandWithFunctionalityParameterKeyMechanicalDifferentialTests")]
    public RoutedUICommandWithFunctionalityParameterKey() { }

    [MirrorVerified("PtcWpfRoutedUICommandWithFunctionalityParameterKeyMechanicalDifferentialTests")]
    protected Dictionary<object, EnumFunctionalityKey> PrimaryParameterToFunctionalityKeyDictionary { get; private set; }

    [MirrorVerified("Release IL")]
    public RoutedUICommandWithFunctionalityParameterKey(string text, string name, Type ownerType, Dictionary<object, EnumFunctionalityKey> parametersToFunctionalityDictionary) : base(text, name, ownerType, EnumFunctionalityKey.GlobalCategory)
    {
        PrimaryParameterToFunctionalityKeyDictionary = parametersToFunctionalityDictionary;
    }

    [MirrorVerified("Release IL")]
    public void SetFunctionalityKeyAccordingParameter(object parameter) => FunctionalityKey = GetFunctionalityKeyAccordingParameter(parameter, true);
}
