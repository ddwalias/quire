#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.Controls.Core;

namespace Quire.Controls.Mathboard;

public partial interface IVerticalWhiteboardSections
{
    int BlockSectionCount { get; }

    event Action SectionsMaybeChanged;

    string GetBlockSectionName(uint sectionIdx);

    IEnumerable<IWhiteboardItem> GetBlockSectionItems(uint sectionIdx);

    double GetBlockSectionMinHeightRatio(uint sectionIdx);
}
