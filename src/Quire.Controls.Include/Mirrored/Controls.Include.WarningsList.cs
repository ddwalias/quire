#nullable disable
#pragma warning disable
using System.Collections.Generic;

namespace Quire.Controls.Include;

public partial class WarningsList
{
    public WarningsList()
    {
    }

    public WarningsList(WarningsList list)
        : base((IEnumerable<WarningOnIncludeWorksheet>)list)
    {
    }
}
