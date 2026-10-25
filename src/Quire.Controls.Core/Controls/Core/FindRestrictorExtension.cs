#nullable disable
using System.Collections.Generic;
using System.Windows.Documents;
using Quire;

namespace Quire.Controls.Core;

public static partial class FindRestrictorExtension
{
    public static bool FindOnce(this IFindRestrictor findRestrictor, string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        if (DebugTools.AssertNotNull(findRestrictor, null).Failed || !findRestrictor.CanSearch)
        {
            return false;
        }
        return findRestrictor.SearchableRegion.FindOncePrivate(text, startFromCaret, direction, options, subscriptPositions);
    }
}
