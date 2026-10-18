#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.IORegions;

public partial class IOItemsAliasManager
{
    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public static void SetInputAliases(DependencyObject element, List<string> aliases) => SetAliases(element, aliases, true);

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public static void SetOutputAliases(DependencyObject element, List<string> aliases) => SetAliases(element, aliases, false);

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    private readonly Dictionary<string, IIOItem> _aliasesToItemsMap;

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public bool IsExist(string alias) => _aliasesToItemsMap.ContainsKey(alias);

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public static event Action<DependencyObject, DependencyPropertyChangedEventArgs> InAliasMapChanged;

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public static event Action<DependencyObject, DependencyPropertyChangedEventArgs> OutAliasMapChanged;
}
