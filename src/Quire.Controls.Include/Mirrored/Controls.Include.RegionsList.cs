#nullable disable
#pragma warning disable
using System.Collections.Generic;
using System.Windows;

namespace Quire.Controls.Include;

public partial class RegionsList
{
    public RegionsList()
    {
    }

    public RegionsList(IEnumerable<UIElement> collection)
        : base(collection)
    {
    }
}
