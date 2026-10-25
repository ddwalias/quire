#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ResizeEventMouseArgs
{
    public readonly DragMode DragMode;

    public readonly FrameworkElement SourceElement;

    public readonly Point StartPoint;

    public readonly DependencyObject OriginalEventSource;

    public readonly bool Automation;

    public IResizer Resizer { internal get; set; }

    internal ResizeEventMouseArgs(RoutedEvent routedEvent, DragMode mode, FrameworkElement element, Point startPosition, DependencyObject originalEventSource, bool automation)
        : base(routedEvent)
    {
        Automation = automation;
        DragMode = mode;
        SourceElement = element;
        StartPoint = startPosition;
        OriginalEventSource = originalEventSource;
    }
}
