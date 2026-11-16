using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quire.Controls.Worksheet;

public partial class SwitchToDraftModeButtonAdorner
{
    private readonly Rectangle _rect;

    public SwitchToDraftModeButtonAdorner(UIElement adornedElement)
        : base(adornedElement, false)
    {
        _rect = new Rectangle
        {
            Fill = new VisualBrush(adornedElement)
        };
        AddVisualChild(_rect);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        _rect.Measure(constraint);
        return base.MeasureOverride(constraint);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _rect.Arrange(new Rect(finalSize));
        return base.ArrangeOverride(finalSize);
    }
}
