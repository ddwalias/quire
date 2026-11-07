#nullable disable
using System.Collections.Generic;
using System.Windows.Documents;

namespace Quire.Controls.Core;

public partial interface ISearchableRegion
{
    bool HighlightAll(string text, FindOptions options);
    bool FindOncePrivate(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions);
}
