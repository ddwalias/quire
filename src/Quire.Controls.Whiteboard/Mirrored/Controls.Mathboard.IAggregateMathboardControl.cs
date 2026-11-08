#nullable disable
#pragma warning disable

using System.Collections.Generic;

namespace Quire.Controls.Mathboard;

public partial interface IAggregateMathboardControl
{
    IEnumerable<ISectionIndicatorInfo> SectionIndicators { get; }
}
