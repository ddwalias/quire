#nullable disable
#pragma warning disable
using System.Collections.Generic;

namespace Quire.Controls.Include;

public partial class ErrorsList
{
    public ErrorsList(ErrorsList list)
        : base((IEnumerable<ErrorOnIncludeWorksheet>)list)
    {
    }

    public ErrorsList()
    {
    }
}
