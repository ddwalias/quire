using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Plot3D.Tools;

public partial class VisualTrackballAdorner
{
    public VisualTrackballAdorner(UIElement adornedElement)
        : base(adornedElement, false)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        Pen pen = new Pen(new SolidColorBrush(Colors.Orange), 1.0);
        Size renderSize = AdornedElement.RenderSize;
        Point point = new Point(renderSize.Width * 0.5, renderSize.Height * 0.5);
        double widthRadius = SpinTrackball.Radius * renderSize.Width * 0.5;
        double heightRadius = SpinTrackball.Radius * renderSize.Height * 0.5;
        drawingContext.DrawEllipse(Brushes.Transparent, pen, point, widthRadius, heightRadius);
        Vector vector = new Vector(widthRadius, 0.0);
        Vector verticalRadiusVector = new Vector(0.0, heightRadius);
        drawingContext.DrawLine(pen, point - vector, point + vector);
        drawingContext.DrawLine(pen, point - verticalRadiusVector, point + verticalRadiusVector);
    }
}
