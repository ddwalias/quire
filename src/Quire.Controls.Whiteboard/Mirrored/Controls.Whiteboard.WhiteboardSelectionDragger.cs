#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

internal partial class WhiteboardSelectionDragger
{
    private readonly WhiteboardControl _container;
    private readonly Point _startPoint;
    private SelectionDraggerRubberBand _selectionDraggerRubberBand;

    public event Action<Rect> DragDone;

    public WhiteboardSelectionDragger(WhiteboardControl container, Point startPosition)
    {
        _container = container;
        _startPoint = startPosition;
        _container.MouseMove += MouseMove;
        _container.MouseUp += MouseUp;
        Mouse.Capture(_container);
    }

    private void MouseUp(object sender, MouseButtonEventArgs e)
    {
        MouseUp(e.GetPosition(_container));
    }

    private void MouseMove(object sender, MouseEventArgs e)
    {
        MouseMove(e.GetPosition(_container));
    }

    private void MouseUp(Point location)
    {
        _container.MouseMove -= MouseMove;
        _container.MouseUp -= MouseUp;
        if (_selectionDraggerRubberBand != null)
        {
            _selectionDraggerRubberBand.Remove();
        }
        DragDone(new Rect(_startPoint, location));
        Mouse.Capture(null);
    }

    internal void MouseMove(Point location)
    {
        if (!WpfUtils.IsSignificantMouseMove(_startPoint, location))
        {
            return;
        }
        if (_selectionDraggerRubberBand == null)
        {
            _selectionDraggerRubberBand = new SelectionDraggerRubberBand(_container);
        }
        _selectionDraggerRubberBand.Bounds = new Rect(_startPoint, location);
        Mouse.Capture(_container);
        _selectionDraggerRubberBand.BringIntoView(new Rect(location, new Size(1.0, 1.0)));
    }
}
