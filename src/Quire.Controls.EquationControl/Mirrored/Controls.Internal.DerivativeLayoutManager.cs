using System;
using System.Windows;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

public partial class DerivativeLayoutManager
{
    private const double EXPRESSION_LEFT_PADDING = 2d;

    public Rect OperatorSymbolRect => _operatorPartItems[0].Rect;

    public Rect VariableOperandRect => _childItems[0].Rect;

    public Rect OrderOperandRect => _childItems[1].Rect;

    public Rect ExpressionOperandRect => _childItems[2].Rect;

    public double DerivativeSymbolVerticalOffset { get; set; }

    public Point BaseOperandsOffset { get; set; }

    public DerivativeLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems, MathNodeDescriptor descriptor)
        : base(childItems, operatorPartItems)
    {
        if (descriptor == null)
        {
            throw new NullReferenceException("descriptor is null");
        }

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
        Point location = new Point(0.0, 0.0);
        Point location2 = new Point(BaseOperandsOffset.X, BaseOperandsOffset.Y + DerivativeSymbolVerticalOffset);
        Point location3 = new Point(location2.X + VariableOperandRect.Width, BaseOperandsOffset.Y);
        double num = _operatorPartItems[0].TextBaselineOffset - _childItems[2].TextBaselineOffset;
        if (num < 0.0)
        {
            location.Y -= num;
            location2.Y -= num;
            location3.Y -= num;
            num = 0.0;
        }

        Point location4 = new Point(location.X + OperatorSymbolRect.Width + EXPRESSION_LEFT_PADDING, num);
        _operatorPartItems[0].Rect.Location = location;
        _childItems[0].Rect.Location = location2;
        _childItems[1].Rect.Location = location3;
        _childItems[2].Rect.Location = location4;
        _baseline = ExpressionOperandRect.Top + _childItems[2].TextBaselineOffset;
    }
}
