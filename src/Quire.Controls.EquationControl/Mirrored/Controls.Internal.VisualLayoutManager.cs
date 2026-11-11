using System.Text;
using System.Windows;
using Quire.Core;

namespace Quire.Controls.Internal;

public abstract partial class VisualLayoutManager
{
    protected LayoutItem[] _childItems;
    protected LayoutItem[] _operatorPartItems;
    protected double _baseline;

    public LayoutItem[] ChildItems => _childItems;

    public LayoutItem[] OperatorPartItems => _operatorPartItems;

    public double TextBaselineOffset => _baseline;

    public Size DesiredSize
    {
        get
        {
            Rect rect = new Rect(0.0, 0.0, 0.0, 0.0);
            foreach (LayoutItem item in _operatorPartItems)
            {
                rect.Union(item.Rect);
            }
            foreach (LayoutItem item in _childItems)
            {
                rect.Union(item.Rect);
            }
            return rect.Size;
        }
    }

    public VisualLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems)
    {
        _childItems = childItems;
        _operatorPartItems = operatorPartItems;
    }

    public abstract void Arrange();

    public virtual void FindClosestChild(Point hitPoint, out LayoutChildType childType, out int childIndex, out Direction childSideHit)
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
        Rect rect = new Rect(new Point(0.0, 0.0), DesiredSize);
        childType = LayoutChildType.OperatorPart;
        childIndex = 0;
        if (hitPoint.X < rect.Left)
        {
            childSideHit = Direction.Left;
        }
        else if (hitPoint.X > rect.Right)
        {
            childSideHit = Direction.Right;
        }
        else if (hitPoint.Y < rect.Top)
        {
            childSideHit = Direction.Up;
        }
        else if (hitPoint.Y > rect.Bottom)
        {
            childSideHit = Direction.Down;
        }
        else
        {
            childSideHit = Direction.Indeterminate;
        }
    }

    protected static void AlignItemAdjacentTo(ref LayoutItem item, LayoutItem referenceItem, double horizontalPadding)
    {
        double y = referenceItem.Rect.Top + (referenceItem.TextBaselineOffset - item.TextBaselineOffset);
        item.Rect.Location = new Point(referenceItem.Rect.Right + horizontalPadding, y);
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Children:");
        for (int i = 0; i < _childItems.Length; i++)
        {
            stringBuilder.AppendFormat("\n  {0}: {1}", i, _childItems[i]);
        }
        stringBuilder.Append("\nOperator Parts:");
        for (int j = 0; j < _operatorPartItems.Length; j++)
        {
            stringBuilder.AppendFormat("\n  {0}: {1}", j, _operatorPartItems[j]);
        }
        return stringBuilder.ToString();
    }
}
