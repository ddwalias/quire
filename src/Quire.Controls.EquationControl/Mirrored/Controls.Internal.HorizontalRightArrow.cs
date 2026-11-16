using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class HorizontalRightArrow : HorizontalFence
{
    private static readonly char[] _operatorChars = new char[4] { '―', '―', '→', '→' };

    public HorizontalRightArrow()
        : base(_operatorChars)
    {
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (!_isFormatValid)
        {
            ReformatComponents();
        }
        FireMeasureOverideCompleted();
        double num = Math.Min(9999.0, constraint.Width);
        Rect bounds = _geometry.Bounds;
        return new Size(num, bounds.Height + VerticalMargin * 2.0);
    }

    protected override void OnRender(DrawingContext dc)
    {
        dc.PushClip(new RectangleGeometry(new Rect(new Point(0.0, 0.0), RenderSize)));
        double num = RenderSize.Width - _components[2].Width;
        dc.DrawText(_components[2], new Point(num, VerticalMargin - _geometry.Bounds.Top));
        num += 0.98;
        while (num > 0.0)
        {
            num -= _components[0].Width;
            dc.DrawText(_components[0], new Point(num, VerticalMargin - _geometry.Bounds.Top));
        }
        dc.Pop();
    }

    public override object Clone()
    {
        HorizontalRightArrow horizontalRightArrow = new HorizontalRightArrow();
        Clone(horizontalRightArrow);
        return horizontalRightArrow;
    }
}
