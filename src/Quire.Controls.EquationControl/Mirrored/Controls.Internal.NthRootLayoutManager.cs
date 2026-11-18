using System;
using System.Windows;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

public partial class NthRootLayoutManager
{
    public Rect RootSymbolRect => _operatorPartItems[0].Rect;

    public Rect OverbarRect => _operatorPartItems[1].Rect;

    public Rect NthExprRect => _childItems[0].Rect;

    public Rect RootExprRect => _childItems[1].Rect;

    public NthRootLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems)
        : base(childItems, operatorPartItems)
    {
        MathNodeDescriptor descriptor = DefaultDescriptorTable.Default["NTHROOT"];
        if (_childItems.Length != descriptor.ChildCount)
        {
            throw new ArgumentException("wrong number of child sizes", "childItems");
        }
    }

    public override void Arrange()
    {
        if (NthExprRect.Height > 0.5 * RootSymbolRect.Height)
        {
            _childItems[0].Rect.Location = new Point(0.0, 0.0);
            _operatorPartItems[0].Rect.Location = new Point(NthExprRect.Width - VisualNthRootOperatorNode.ROOT_SYMBOL_X_POS_ADJUSTMENT, NthExprRect.Height - VisualNthRootOperatorNode.ROOT_SYMBOL_Y_POS_ADJUSTMENT * RootSymbolRect.Height + VisualNthRootOperatorNode.NTH_EXPR_BOTTOM_PADDING);
        }
        else
        {
            _childItems[0].Rect.Location = new Point(0.0, VisualNthRootOperatorNode.ROOT_SYMBOL_Y_POS_ADJUSTMENT * RootSymbolRect.Height - NthExprRect.Height - 0.05 * RootSymbolRect.Height);
            _operatorPartItems[0].Rect.Location = new Point(NthExprRect.Width - VisualNthRootOperatorNode.ROOT_SYMBOL_X_POS_ADJUSTMENT, 0.0);
        }
        _operatorPartItems[1].Rect.Location = new Point(RootSymbolRect.Right - VisualNthRootOperatorNode.ROOT_SYMBOL_RIGHT_MARGIN, RootSymbolRect.Top);
        _childItems[1].Rect.Location = new Point(RootSymbolRect.Right, OverbarRect.Bottom + VisualNthRootOperatorNode.ROOT_EXPR_TOP_PADDING);
        _baseline = RootExprRect.Top + _childItems[1].TextBaselineOffset;
    }
}
