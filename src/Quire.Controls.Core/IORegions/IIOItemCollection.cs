#nullable disable
using System;

namespace Quire.Controls.Core.IORegions;

public partial interface IIOItemCollection
{
    event Action AliasChanged;
    IIOItem GetItem(string alias, bool compareAliasCaseInsensitively);
    void ResetAlias(IIOItem item, string oldAlias, string newAlias);
}
