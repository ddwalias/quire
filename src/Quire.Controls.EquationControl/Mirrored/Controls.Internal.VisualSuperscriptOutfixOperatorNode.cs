using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualSuperscriptOutfixOperatorNode
{
    public VisualSuperscriptOutfixOperatorNode(DelegateFunction0<Fence> leftFenceFactory, DelegateFunction0<Fence> rightFenceFactory)
        : this(true, leftFenceFactory, rightFenceFactory)
    {
    }

    public VisualSuperscriptOutfixOperatorNode(bool horizontal, DelegateFunction0<Fence> leftFenceFactory, DelegateFunction0<Fence> rightFenceFactory)
        : base(horizontal, leftFenceFactory, rightFenceFactory)
    {
    }

    public static VisualSuperscriptOutfixOperatorNode CreateHorizontalBrackets()
    {
        return new VisualSuperscriptOutfixOperatorNode(true, VerticalFence.CreateLeftAngleBracket, VerticalFence.CreateRightAngleBracket);
    }

    public static VisualSuperscriptOutfixOperatorNode CreateVerticalBrackets()
    {
        return new VisualSuperscriptOutfixOperatorNode(false, HorizontalFence.CreateLowerAngleBracket, HorizontalFence.CreateUpperAngleBracket);
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        if (!HideFromClicks)
        {
            Point point = TransformToDescendant((Visual)LeftFence).Transform(hitPoint);
            if (Horizontal)
            {
                double actualWidth = (LeftFence as Fence).ActualWidth;
                if (point.X < actualWidth)
                {
                    return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Left, hitPoint);
                }
            }
            else
            {
                double actualHeight = (LeftFence as Fence).ActualHeight;
                if (point.Y > 0.0 && point.Y < actualHeight)
                {
                    return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Left, hitPoint);
                }
            }
        }
        GeneralTransform generalTransform = TransformToDescendant(Child as VisualMathNode);
        return (Child as VisualMathNode).GetNodeAtPoint(generalTransform.Transform(hitPoint));
    }
}
