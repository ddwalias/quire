using System.Windows;
using Quire.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

internal partial class GradientLayoutManager : VisualLayoutManager
{
    private const int VariableListOperandIndex = 0;

    private const int FunctionOperandIndex = 1;

    private const int OperatorIndex = 0;

    private const double VariableListHorizontalPadding = 3.0;

    public Rect VariableListRect => _childItems[0].Rect;

    public Rect FunctionRect => _childItems[1].Rect;

    public Rect GradientSymbolRect => _operatorPartItems[0].Rect;

    public GradientLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems)
        : base(childItems, operatorPartItems)
    {
        MathNodeDescriptor descriptor = DefaultDescriptorTable.Default["GRADIENT"];
        DebugTools.DevAssert(_childItems.Length == descriptor.ChildCount, null);
        DebugTools.DevAssert(_operatorPartItems.Length == descriptor.NavigablePartCount, null);
    }

    public override void Arrange()
    {
        Point location = new Point(0.0, 0.0);
        Point location2 = new Point(GradientSymbolRect.Width, location.Y + GradientSymbolRect.Height / 2.0);
        double num = _operatorPartItems[0].TextBaselineOffset - _childItems[1].TextBaselineOffset;
        if (num < 0.0)
        {
            location.Y -= num;
            location2.Y -= num;
            num = 0.0;
        }

        Point location3 = new Point(location2.X + VariableListRect.Width + 3.0, num);
        _childItems[0].Rect.Location = location2;
        _operatorPartItems[0].Rect.Location = location;
        _childItems[1].Rect.Location = location3;
        _baseline = GradientSymbolRect.Top + _operatorPartItems[0].TextBaselineOffset;
    }

    private static Direction FindClosestChildDirection(Point hitPoint, Rect rect)
    {
        Direction direction;
        if (hitPoint.X < rect.Left)
        {
            direction = Direction.Left;
        }
        else if (rect.Contains(hitPoint))
        {
            direction = Direction.Indeterminate;
        }
        else
        {
            direction = (hitPoint.Y < rect.Top) ? Direction.Up : Direction.Down;
        }

        return direction;
    }

    public override void FindClosestChild(Point hitPoint, out LayoutChildType childType, out int childIndex, out Direction childSideHit)
    {
        if (hitPoint.X < VariableListRect.Left)
        {
            childType = LayoutChildType.OperatorPart;
            childIndex = 0;
            childSideHit = FindClosestChildDirection(hitPoint, GradientSymbolRect);
            return;
        }

        if (hitPoint.X < FunctionRect.Left)
        {
            childType = LayoutChildType.Child;
            childIndex = 0;
            childSideHit = FindClosestChildDirection(hitPoint, VariableListRect);
            return;
        }

        if (hitPoint.X < FunctionRect.Right)
        {
            childType = LayoutChildType.Child;
            childIndex = 1;
            childSideHit = FindClosestChildDirection(hitPoint, FunctionRect);
        }
        else
        {
            childType = LayoutChildType.Child;
            childIndex = 1;
            childSideHit = Direction.Right;
        }
    }
}
