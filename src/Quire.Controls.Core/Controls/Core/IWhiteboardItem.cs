#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IWhiteboardItem
{
    double VerticalAlignmentOffset { get; }
    bool IsEmpty { get; }
    event Action<IWhiteboardItem> OnGotFocusEvent;
    event Action<IWhiteboardItem, Direction> RequestRemove;
    void Activate(bool isActive);
    Vector? NavigateIntoItem(Direction direction);
    void SimulateMouseClick(Point location, int numberOfClicks);
}
