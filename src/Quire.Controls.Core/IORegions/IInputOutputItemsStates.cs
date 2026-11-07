#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core.IORegions;

public partial interface IInputOutputItemsStates
{
    IList<IScalarItemWithOriginalState> InputItems { get; }
    IList<IScalarItem> OutputItems { get; }
}
