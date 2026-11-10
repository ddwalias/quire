using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualBinaryWrappOperatorNodeEditor
{
    private void DrawSymbolGeometry(DrawingContext drawingContext, UIElement signElement, Geometry signGeometry)
    {
        Point point = signElement.TranslatePoint(new Point(0.0, 0.0), AdornedElement);
        signGeometry.Transform = new TranslateTransform(point.X, point.Y);
        drawingContext.DrawGeometry(_caretBrush, null, signGeometry);
    }
}
