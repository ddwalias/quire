#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core;

public partial interface ISelectionDragger
{
    event Action<Point> DragDone;
}
