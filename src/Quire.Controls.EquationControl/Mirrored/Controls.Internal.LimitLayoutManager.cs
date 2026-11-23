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

    public override void FindClosestChild(Point hitPoint, out LayoutChildType childType, out int childIndex, out Direction childSideHit)
    {
        for (int i = 0; i < _childItems.Length; i++)
        {
            if (_childItems[i].Rect.Contains(hitPoint))
            {
                childType = LayoutChildType.Child;
                childIndex = i;
                childSideHit = Direction.Indeterminate;
                return;
            }
        }

        for (int j = 0; j < _operatorPartItems.Length; j++)
        {
            if (_operatorPartItems[j].Rect.Contains(hitPoint))
            {
                childType = LayoutChildType.OperatorPart;
                childIndex = j;
                childSideHit = Direction.Indeterminate;
                return;
            }
        }

        if (hitPoint.X < 0.0)
        {
            childType = LayoutChildType.OperatorPart;
            childIndex = 0;
            childSideHit = Direction.Left;
            return;
        }

        if (hitPoint.X > LimitExpressionRect.Right)
        {
            childType = LayoutChildType.Child;
            childIndex = 3;
            childSideHit = Direction.Right;
            return;
        }

        if (hitPoint.X < 5.0)
        {
            if (LimitSymbolRect.Left > 5.0)
            {
                if (hitPoint.Y < LimitVariableRect.Top)
                {
                    childType = LayoutChildType.OperatorPart;
                    childIndex = 0;
                    childSideHit = Direction.Left;
                    return;
                }
            }
        }

        if (hitPoint.Y < LimitSymbolRect.Top)
        {
            if (hitPoint.X < LimitExpressionRect.Left)
            {
                childType = LayoutChildType.OperatorPart;
                childIndex = 0;
                childSideHit = Direction.Up;
                return;
            }
        }

        for (int k = 0; k < _childItems.Length; k++)
        {
            if (hitPoint.Y < _childItems[k].Rect.Top && IsBetween(hitPoint.X, _childItems[k].Rect.Left, _childItems[k].Rect.Right))
            {
                childType = LayoutChildType.Child;
                childIndex = k;
                childSideHit = Direction.Up;
                return;
            }

            if (hitPoint.Y > _childItems[k].Rect.Bottom && IsBetween(hitPoint.X, _childItems[k].Rect.Left, _childItems[k].Rect.Right))
            {
                childType = LayoutChildType.Child;
                childIndex = k;
                childSideHit = Direction.Down;
                return;
            }
        }

        childType = LayoutChildType.OperatorPart;
        childIndex = 0;
        childSideHit = Direction.Down;
    }

    private bool IsBetween(double given, double left, double right)
    {
        if (left < given)
        {
            return given < right;
        }

        return false;
    }
}
