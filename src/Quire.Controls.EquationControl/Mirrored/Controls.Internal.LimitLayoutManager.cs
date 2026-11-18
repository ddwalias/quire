using System;
using System.Windows;

namespace Quire.Controls.Internal;

internal partial class LimitLayoutManager
{
    public Rect LimitVariableRect => _childItems[0].Rect;

    public Rect LimitVariableExpressionRect => _childItems[1].Rect;

    public Rect LimitSideRect => _childItems[2].Rect;

    public Rect LimitExpressionRect => _childItems[3].Rect;

    public Rect LimitSymbolRect => _operatorPartItems[0].Rect;

    public Rect LimitArrowRect => _operatorPartItems[1].Rect;

    public LimitLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems)
        : base(childItems, operatorPartItems)
    {
    }

    public override void Arrange()
    {
        Point p0 = new Point(0.0, 0.0);
        Point p1 = new Point(p0.X + (LimitSymbolRect.Width - LimitArrowRect.Width) / 2.0, p0.Y + LimitSymbolRect.Height);
        Point p2 = new Point(p1.X - LimitVariableRect.Width, p1.Y);
        Point p3 = new Point(p1.X + LimitArrowRect.Width, p1.Y);
        Point p4 = new Point(p3.X + LimitVariableExpressionRect.Width, p3.Y - LimitSideRect.Height / 2.0);
        double baselineDelta = _operatorPartItems[0].TextBaselineOffset - _childItems[3].TextBaselineOffset;
        Point p6 = new Point(p4.X + LimitSideRect.Width, baselineDelta);
        if (p2.X < 0.0)
        {
            double shiftX = -p2.X;
            p0.X += shiftX;
            p1.X += shiftX;
            p2.X += shiftX;
            p3.X += shiftX;
            p4.X += shiftX;
            p6.X += shiftX;
        }

        if (p6.Y < 0.0 || p4.Y < 0.0)
        {
            double shiftY = -Math.Min(p6.Y, p4.Y);
            p0.Y += shiftY;
            p1.Y += shiftY;
            p2.Y += shiftY;
            p3.Y += shiftY;
            p4.Y += shiftY;
            p6.Y += shiftY;
        }

        _operatorPartItems[0].Rect.Location = p0;
        _operatorPartItems[1].Rect.Location = p1;
        _childItems[0].Rect.Location = p2;
        _childItems[1].Rect.Location = p3;
        _childItems[2].Rect.Location = p4;
        _childItems[3].Rect.Location = p6;
        _baseline = LimitExpressionRect.Y + _childItems[3].TextBaselineOffset;
    }
}
