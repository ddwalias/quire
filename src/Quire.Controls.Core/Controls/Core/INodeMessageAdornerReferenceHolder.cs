#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core;

public partial interface INodeMessageAdornerReferenceHolder
{
    IEnumerable<NodeMessageAdorner> NodeMessageAdorners { get; }
}
