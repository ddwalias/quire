using System.Windows;
using System.Windows.Shapes;
using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class CaretAdorner : VisualAdorner
{
    private readonly UIElement _adornedElement;

    public readonly Direction CaretSide;

    public readonly Caret _caret = new Caret();

    public CaretAdorner(UIElement adornedElement, Direction caretSide)
        : base(adornedElement, false)
    {
        _adornedElement = adornedElement;
        CaretSide = caretSide;
        AddVisualChild(_caret.Line);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        double num = (CaretSide == Direction.Left) ? 0.0 : _adornedElement.DesiredSize.Width;
        _caret.Line.BeginInit();
        _caret.Line.X1 = num;
        _caret.Line.X2 = num;
        _caret.Line.Y1 = 0.0;
        _caret.Line.Y2 = _adornedElement.DesiredSize.Height;
        _caret.Line.EndInit();
        _caret.Line.Measure(constraint);
        return _caret.Line.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Size val = finalSize;
        val.Width += 1.0;
        _caret.Line.Arrange(new Rect(new Point(0.0, 0.0), val));
        _caret.Line.Arrange(new Rect(finalSize));
        return finalSize;
    }

    public void SetCaretType(bool isReadWrite)
    {
        if (isReadWrite)
        {
            _caret.Color = MathcadSettings.Instance.CaretColor;
        }
        else
        {
            _caret.Color = MathcadSettings.Instance.ReadOnlyCaretColor;
        }
    }
}
