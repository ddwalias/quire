using System;
using System.Windows;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

public partial class RangedPrefixLayoutManager
{
    private const double PRINCIPAL_LEFT_PADDING = 2d;
    private const double RANGEBOUND_VERTICAL_PADDING = 1d;
    private const double RANGEBOUND_HORIZONTAL_PADDING = 3d;

    public Rect LowerRangeBoundRect => _childItems[0].Rect;

    public Rect UpperRangeBoundRect => _childItems[1].Rect;

    public Rect PrincipalOperandRect => _childItems[2].Rect;

    public Rect OperatorSymbolRect => _operatorPartItems[0].Rect;

    public RangedPrefixLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems)
        : base(childItems, operatorPartItems)
    {
        MathNodeDescriptor descriptor = DefaultDescriptorTable.Default["SUMMATION"];
        if (_childItems.Length != descriptor.ChildCount)
        {
            throw new ArgumentException("wrong number of child sizes", "childItems");
        }

        if (_operatorPartItems.Length != descriptor.NavigablePartCount)
        {
            throw new ArgumentException("wrong number of operator part sizes", "operatorPartItems");
        }
    }

    public override void Arrange()
    {
        double upperTop = ChildItems[2].TextBaselineOffset - _operatorPartItems[0].TextBaselineOffset - 1.0 - UpperRangeBoundRect.Height;
        if (upperTop < 0.0)
        {
            upperTop = 0.0;
        }

        Point p1 = new Point(0.0, upperTop);
        Point p2 = new Point(0.0, upperTop + UpperRangeBoundRect.Height + 1.0);
        Point p3 = new Point(0.0, p2.Y + OperatorSymbolRect.Height + 1.0);
        double rangeWidth = Math.Max(UpperRangeBoundRect.Width, LowerRangeBoundRect.Width);
        // Written as if/else: the equivalent conditional compiles to a different branch shape than Mathcad's IL.
        double operatorLeft;
        if (rangeWidth > OperatorSymbolRect.Width)
        {
            operatorLeft = rangeWidth + 6.0;
        }
        else
        {
            operatorLeft = OperatorSymbolRect.Width + 2.0;
        }

        p1.X = (operatorLeft - UpperRangeBoundRect.Width) / 2.0;
        p2.X = (operatorLeft - OperatorSymbolRect.Width) / 2.0;
        p3.X = (operatorLeft - LowerRangeBoundRect.Width) / 2.0;
        _childItems[0].Rect.Location = p3;
        _operatorPartItems[0].Rect.Location = p2;
        _childItems[1].Rect.Location = p1;
        double principalX = operatorLeft;
        double principalY = OperatorSymbolRect.Top + _operatorPartItems[0].TextBaselineOffset - _childItems[2].TextBaselineOffset;
        Point p8 = new Point(principalX, principalY);
        _childItems[2].Rect.Location = p8;
        _baseline = OperatorSymbolRect.Top + _operatorPartItems[0].TextBaselineOffset;
    }
}
