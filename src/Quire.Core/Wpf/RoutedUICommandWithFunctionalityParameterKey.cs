#nullable disable
using Quire.FunctionalitiesLimitation;
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;
using System.Linq;
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
    public RoutedUICommandWithFunctionalityParameterKey(string text, string name, Type ownerType, IEqualityComparer<object> comparer = null) : base(text, name, ownerType, EnumFunctionalityKey.GlobalCategory)
    {
        PrimaryParameterToFunctionalityKeyDictionary = comparer == null ? new Dictionary<object, EnumFunctionalityKey>() : new Dictionary<object, EnumFunctionalityKey>(comparer);
    }

    [MirrorVerified("Release IL")]
    public void SetFunctionalityKeyAccordingParameter(object parameter) => FunctionalityKey = GetFunctionalityKeyAccordingParameter(parameter, true);

    [MirrorVerified("Release IL")]
    public virtual IEnumerable<Dictionary<object, EnumFunctionalityKey>> ParameterToFunctionalityKeyDictionaryList
    {
        get
        {
            yield return PrimaryParameterToFunctionalityKeyDictionary;
        }
    }

    public bool ContainsParameter(object parameter) => ParameterToFunctionalityKeyDictionaryList.Any((Dictionary<object, EnumFunctionalityKey> dict) => dict.ContainsKey(parameter));

    public void AddFunctionalityKeyAccordingParameter(object parameter, EnumFunctionalityKey functionalityKey)
    {
        if (!ContainsParameter(parameter))
        {
            PrimaryParameterToFunctionalityKeyDictionary.Add(parameter, functionalityKey);
        }
    }

    public EnumFunctionalityKey GetFunctionalityKeyAccordingParameter(object parameter, bool failAssertionIfNotFound = true)
    {
        EnumFunctionalityKey funcKey = EnumFunctionalityKey.GlobalCategory;
        if (!ParameterToFunctionalityKeyDictionaryList.Any((Dictionary<object, EnumFunctionalityKey> dict) => dict.TryGetValue(parameter, out funcKey)))
        {
            funcKey = EnumFunctionalityKey.GlobalCategory;
            if (failAssertionIfNotFound)
            {
                DebugTools.FailAssertion("Parameter \"{0}\" not found in the functionality key dictionary of \"{1}\" command.", parameter, Name);
            }
        }
        return funcKey;
    }
}
