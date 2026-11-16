using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class Overline : HorizontalFence
{
    private static readonly char[] _operatorChars = new char[4] { '‾', '‾', '‾', '‾' };

    public Overline()
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
        double num = 0.0;
        while (num < RenderSize.Width)
        {
            dc.DrawText(_components[0], new Point(num, VerticalMargin - _geometry.Bounds.Top));
            num += _components[0].Width;
        }
        dc.Pop();
    }

    public override object Clone()
    {
        Overline overline = new Overline();
        Clone(overline);
        return overline;
    }
}
