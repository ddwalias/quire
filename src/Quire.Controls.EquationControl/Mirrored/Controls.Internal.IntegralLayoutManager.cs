using System;
using System.Windows;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

public partial class IntegralLayoutManager
{
    private const double INTEGRAND_LEFT_PADDING = 2d;
    private const double INTEGRAND_RIGHT_PADDING = 2d;
    private const double LIMITBOUND_ADJACENT_HORIZONTAL_PADDING = 3d;
    private const double LIMITBOUND_HORIZONTAL_PADDING = 1d;
    private const double LIMITBOUND_VERTICAL_PADDING = 0.5d;
    private readonly bool _adjacentIntegral;
    private readonly double _adjacentUpperLimitHeight;
    private readonly double _adjacentLowerLimitHeight;

    public Rect LowerLimitRect => _childItems[0].Rect;

    public Rect UpperLimitRect => _childItems[1].Rect;

    public Rect IntegrandRect => _childItems[2].Rect;

    public Rect VariableRect => _childItems[3].Rect;

    public Rect IntegralSymbolRect => _operatorPartItems[0].Rect;

    public Rect VariablePrefixRect => _operatorPartItems[1].Rect;

    public IntegralLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems)
        : this(childItems, operatorPartItems, false, 0.0, 0.0)
    {
    }

    public IntegralLayoutManager(LayoutItem[] childItems, LayoutItem[] operatorPartItems, bool adjacentIntegral, double adjacentUpperLimitHeight, double adjacentLowerLimitHeight)
        : base(childItems, operatorPartItems)
    {
        MathNodeDescriptor descriptor = DefaultDescriptorTable.Default["INTEGRAL"];
        if (_childItems.Length != descriptor.ChildCount)
        {
            throw new ArgumentException("wrong number of child sizes", "childItems");
        }

        if (_operatorPartItems.Length != descriptor.NavigablePartCount)
        {
            throw new ArgumentException("wrong number of operator part sizes", "operatorPartItems");
        }

        _adjacentIntegral = adjacentIntegral;
        _adjacentUpperLimitHeight = adjacentUpperLimitHeight;
        _adjacentLowerLimitHeight = adjacentLowerLimitHeight;
    }

    public override void Arrange()
    {
        double heightDelta = _adjacentUpperLimitHeight - _adjacentLowerLimitHeight;
        double upperTop = _adjacentUpperLimitHeight - UpperLimitRect.Height;
        if (upperTop < 0.0)
        {
            upperTop = 0.0;
        }

        Point p2 = new Point(0.0, upperTop);
        Point p3 = new Point(0.0, p2.Y + UpperLimitRect.Height + LIMITBOUND_VERTICAL_PADDING);
        Point p4 = new Point(0.0, p3.Y + IntegralSymbolRect.Height + LIMITBOUND_VERTICAL_PADDING);
        double halfIntegralWidth = IntegralSymbolRect.Width / 2.0;
        p2.X = p3.X + halfIntegralWidth + LIMITBOUND_HORIZONTAL_PADDING;
        p4.X = p3.X + LIMITBOUND_HORIZONTAL_PADDING;
        _childItems[0].Rect.Location = p4;
        _operatorPartItems[0].Rect.Location = p3;
        _childItems[1].Rect.Location = p2;
        double integralRight = IntegralSymbolRect.Right + INTEGRAND_LEFT_PADDING;
        if (_adjacentIntegral)
        {
            double adjacentWidth = Math.Max(LowerLimitRect.Width, UpperLimitRect.Width) + 6.0;
            integralRight = Math.Max(IntegralSymbolRect.Width, adjacentWidth);
        }

        double integrandY = IntegralSymbolRect.Top + (IntegralSymbolRect.Height - IntegrandRect.Height - heightDelta) / 2.0;
        Point p8 = new Point(integralRight, integrandY);
        _childItems[2].Rect.Location = p8;
        AlignItemAdjacentTo(ref _operatorPartItems[1], _childItems[2], 2.0);
        AlignItemAdjacentTo(ref _childItems[3], _operatorPartItems[1], 0.0);
        _baseline = IntegrandRect.Top + _childItems[2].TextBaselineOffset;
    }
}
