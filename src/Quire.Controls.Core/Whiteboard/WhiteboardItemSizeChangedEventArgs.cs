#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core.Whiteboard;

public partial class WhiteboardItemSizeChangedEventArgs : EventArgs
{
    public Size PreviousSize { get; private set; }

    public Size NewSize { get; private set; }

    public WhiteboardItemSizeChangedEventArgs(SizeChangedEventArgs origArgs)
    {
        PreviousSize = origArgs.PreviousSize;
        NewSize = origArgs.NewSize;
    }

    public WhiteboardItemSizeChangedEventArgs(Size prevSize, Size newSize)
    {
        PreviousSize = prevSize;
        NewSize = newSize;
    }
}
