using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualBinaryWrappOperatorNodeEditor
{
    private readonly VisualMultiLineWrappingBinaryOperatorNode _wrappingOperatorNode;

    public VisualBinaryWrappOperatorNodeEditor(VisualMultiLineWrappingBinaryOperatorNode editNode, VisualMathRegionNode region)
        : base(editNode, region, isReadonly: false, addornOperator: true)
    {
        _wrappingOperatorNode = editNode;
    }

    public override void ShowOperatorHighlight(bool show)
    {
        _wrappingOperatorNode.WrappingSign.Visibility = show ? Visibility.Hidden : Visibility.Visible;
        base.ShowOperatorHighlight(show);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (!_showOperatorHighlight)
        {
            return;
        }

        DrawSymbolGeometry(drawingContext, _wrappingOperatorNode.OperatorElement, _operatorGeometry);
        Geometry signGeometry = _wrappingOperatorNode.WrappingSign.Geometry.Clone();
        DrawSymbolGeometry(drawingContext, _wrappingOperatorNode.WrappingSign, signGeometry);
    }

    private void DrawSymbolGeometry(DrawingContext drawingContext, UIElement signElement, Geometry signGeometry)
    {
        Point point = signElement.TranslatePoint(new Point(0.0, 0.0), AdornedElement);
        signGeometry.Transform = new TranslateTransform(point.X, point.Y);
        drawingContext.DrawGeometry(_caretBrush, null, signGeometry);
    }

    protected override void OnOperatorSelectedChanged(bool selected)
    {
        _wrappingOperatorNode.WrappingSign.Visibility = selected ? Visibility.Hidden : Visibility.Visible;
        base.OnOperatorSelectedChanged(selected);
    }
}
