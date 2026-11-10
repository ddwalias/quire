using System;
using System.Windows;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

public partial class ConjugateLayoutManager
{
    private const double OPERATOR_SYMBOL_BOTTOM_PADDING = 4d;

    public ConjugateLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems)
        : base(childItems, operatorPartItems)
    {
        MathNodeDescriptor descriptor = DefaultDescriptorTable.Default["CONJUGATE"];
        if (_childItems.Length != descriptor.ChildCount)
        {
            throw new ArgumentException("wrong number of child sizes", "childItems");
        }

        if (_operatorPartItems.Length != descriptor.NavigablePartCount)
        {
            throw new ArgumentException("wrong number of operator part sizes", "operatorPartItems");
        }
    }

    public Rect OperatorSymbolRect => _operatorPartItems[0].Rect;

    public Rect ConjugatedExprRect => _childItems[0].Rect;

    public override void Arrange()
    {
        _operatorPartItems[0].Rect.Location = new Point(0.0, 0.0);
        _childItems[0].Rect.Location = new Point((_operatorPartItems[0].Rect.Width - _childItems[0].Rect.Width) / 2.0, OPERATOR_SYMBOL_BOTTOM_PADDING);
        _baseline = ConjugatedExprRect.Top + _childItems[0].TextBaselineOffset;
    }
}
