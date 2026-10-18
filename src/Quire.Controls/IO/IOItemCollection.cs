#nullable disable
using System.Windows;
using Quire.Controls.Core.IORegions;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class IOItemCollection
{
    [MirrorVerified("PtcControlsIOIOItemCollectionMechanicalDifferentialTests")]
    private readonly IOItemsAliasManager _aliasManager;

    [MirrorVerified("PtcControlsIOIOItemCollectionMechanicalDifferentialTests")]
    public IIOItem GetItem(string alias, bool compareAliasCaseInsensitively) => _aliasManager.GetItem(alias, compareAliasCaseInsensitively);

    [MirrorVerified("PtcControlsIOIOItemCollectionMechanicalDifferentialTests")]
    internal void HandleTaggedRegionAdded(DependencyObject region) => CreateIOItems(region);

    [MirrorVerified("PtcControlsIOIOItemCollectionMechanicalDifferentialTests")]
    public event Action AliasChanged;
}
