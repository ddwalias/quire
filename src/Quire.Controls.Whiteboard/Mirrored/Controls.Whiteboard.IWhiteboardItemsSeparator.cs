#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Whiteboard;

public partial interface IWhiteboardItemsSeparator
{
    bool CanSeparate();

    IDictionary<IWhiteboardItem, Vector> Separate();
}
