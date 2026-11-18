using System;
using System.Windows;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

public partial class SymbolicEvaluationLayoutManager
{
    internal const double SYMBOL_HORIZONTAL_PADDING = 4d;
    internal const double SYMBOL_VERTICAL_PADDING_ABOVE = -4d;

    public Rect ExpressionRect => _childItems[0].Rect;

    public Rect KeywordsRect => _childItems[1].Rect;

    public Rect ResultRect => _childItems[2].Rect;

    public Rect SymEvalSymbolRect => _operatorPartItems[0].Rect;

    public SymbolicEvaluationLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems)
        : base(childItems, operatorPartItems)
    {
        MathNodeDescriptor descriptor = DefaultDescriptorTable.Default["SYMBOLIC_EVALUATION"];
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
        double baseline = Math.Max(Math.Max(ChildItems[0].TextBaselineOffset, OperatorPartItems[0].TextBaselineOffset), ChildItems[2].TextBaselineOffset);
        Point p1 = new Point(0.0, baseline - ChildItems[0].TextBaselineOffset);
        Point p2 = new Point(0.0, baseline - OperatorPartItems[0].TextBaselineOffset);
        Point p3 = new Point(0.0, baseline - ChildItems[2].TextBaselineOffset);
        Point p4 = new Point(0.0, p2.Y - ChildItems[1].Rect.Height - SYMBOL_VERTICAL_PADDING_ABOVE);
        if (p4.Y < 0.0)
        {
            p1.Y -= p4.Y;
            p2.Y -= p4.Y;
            p3.Y -= p4.Y;
            p4.Y = 0.0;
        }

        if (ChildItems[1].Rect.Width <= OperatorPartItems[0].Rect.Width)
        {
            p2.X = ChildItems[0].Rect.Width + SYMBOL_HORIZONTAL_PADDING;
            p3.X = p2.X + OperatorPartItems[0].Rect.Width + SYMBOL_HORIZONTAL_PADDING;
            p4.X = p2.X + (OperatorPartItems[0].Rect.Width - ChildItems[1].Rect.Width) / 2.0;
        }
        else
        {
            p4.X = ChildItems[0].Rect.Width + SYMBOL_HORIZONTAL_PADDING;
            p3.X = p4.X + ChildItems[1].Rect.Width + SYMBOL_HORIZONTAL_PADDING;
            p2.X = p4.X + (ChildItems[1].Rect.Width - OperatorPartItems[0].Rect.Width) / 2.0;
        }

        ChildItems[0].Rect.Location = p1;
        ChildItems[1].Rect.Location = p4;
        ChildItems[2].Rect.Location = p3;
        OperatorPartItems[0].Rect.Location = p2;
        _baseline = SymEvalSymbolRect.Top + OperatorPartItems[0].TextBaselineOffset;
    }
}
