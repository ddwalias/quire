using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class VisualDerivativeSymbolNode : VisualMathNode, IGeometryProvider
{
    private const double FRACTIONBAR_VERTICAL_PADDING = 2.0;

    private const double FRACTIONBAR_SIDE_OVERHANG = 2.0;

    private double _baseline;

    private HorizontalFence _fractionBar;

    private VisualSymbolNode _upperDerivativeSymbol;

    private VisualSymbolNode _lowerDerivativeSymbol;

    private VisualSymbolNode _orderSymbol;

    public override double EffectiveBaselineY
    {
        get
        {
            Size desiredSize = DesiredSize;
            return desiredSize.Height;
        }
    }

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _baseline;

    public double DerivativeSymbolVerticalOffset { get; set; }

    public Size BaseOperandsSize { get; set; }

    public Point BaseOperandsOffset { get; set; }

    public Geometry Geometry
    {
        get
        {
            Geometry geometry = _fractionBar.Geometry;
            Geometry geometry2 = _upperDerivativeSymbol.Geometry;
            Geometry geometry3 = _lowerDerivativeSymbol.Geometry;
            Geometry geometry4 = _orderSymbol.Geometry;
            Point val = _fractionBar.TransformToAncestor(this).Transform(new Point(VisualOffset.X, VisualOffset.Y));
            geometry.Transform = new TranslateTransform(val.X, val.Y);
            Point val2 = _upperDerivativeSymbol.TransformToAncestor(this).Transform(new Point(VisualOffset.X, VisualOffset.Y));
            geometry2.Transform = new TranslateTransform(val2.X, val2.Y);
            Point val3 = _lowerDerivativeSymbol.TransformToAncestor(this).Transform(new Point(VisualOffset.X, VisualOffset.Y));
            geometry3.Transform = new TranslateTransform(val3.X, val3.Y);
            if (!geometry4.IsEmpty())
            {
                Point val4 = _orderSymbol.TransformToAncestor(this).Transform(new Point(VisualOffset.X, VisualOffset.Y));
                geometry4.Transform = new TranslateTransform(val4.X, val4.Y);
            }
            return new GeometryGroup
            {
                Children = { geometry, geometry2, geometry3, geometry4 }
            };
        }
    }

    public VisualDerivativeSymbolNode(char? derivativeSymbol)
    {
        char symbol = (DebugTools.DevAssert(derivativeSymbol.HasValue, null).Passed ? derivativeSymbol.Value : 'd');
        _fractionBar = new HorizontalFence('―');
        _upperDerivativeSymbol = new VisualSymbolNode(symbol);
        _lowerDerivativeSymbol = new VisualSymbolNode(symbol);
        _orderSymbol = new VisualSpacer();
        InitializeChildren();
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        if (hitPoint.X < 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
        }
        if (hitPoint.X >= BaseOperandsOffset.X && hitPoint.Y >= BaseOperandsOffset.Y)
        {
            return null;
        }
        if (hitPoint.Y < ActualWidth)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
        }
        return null;
    }

    private void InitializeChildren()
    {
        Children.Clear();
        SetOrderSymbolFontSize(MathFormat);
        Children.Add(_fractionBar);
        Children.Add(_upperDerivativeSymbol);
        Children.Add(_lowerDerivativeSymbol);
        Children.Add(_orderSymbol);
    }

    private void SetOrderSymbolFontSize(IMathFormat format)
    {
        _orderSymbol.MathFormat.FontSize = VisualMathNode.FontManager.GetScaledFontSize(format, FontConstants.ScriptFontSizeRatio);
    }

    public void SetOrderSymbolShowOperatorOnlyWhenActive(bool show)
    {
        _orderSymbol.ShowOperatorOnlyWhenActive = show;
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        _upperDerivativeSymbol.MeasureMathNode(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _lowerDerivativeSymbol.MeasureMathNode(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _orderSymbol.MeasureMathNode(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _fractionBar.MeasureMathNode(new Size(_lowerDerivativeSymbol.DesiredSize.Width + BaseOperandsSize.Width + 4.0, double.PositiveInfinity));
        BaseOperandsOffset = new Point(2.0 + _lowerDerivativeSymbol.DesiredSize.Width, DerivativeSymbolVerticalOffset + _upperDerivativeSymbol.DesiredSize.Height + _fractionBar.DesiredSize.Height + 4.0);
        _baseline = DerivativeSymbolVerticalOffset + _upperDerivativeSymbol.DesiredSize.Height + 2.0 + _fractionBar.DesiredSize.Height / 2.0 - Font.BaselineAdjustment;
        return new Size(_fractionBar.DesiredSize.Width, BaseOperandsOffset.Y + BaseOperandsSize.Height);
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        double width = _fractionBar.DesiredSize.Width;
        Point point = new Point((width - _upperDerivativeSymbol.DesiredSize.Width - _orderSymbol.DesiredSize.Width) / 2.0, DerivativeSymbolVerticalOffset);
        Point point2 = new Point(0.0, point.Y + _upperDerivativeSymbol.DesiredSize.Height + 2.0);
        Point point3 = new Point((width - _lowerDerivativeSymbol.DesiredSize.Width - BaseOperandsSize.Width) / 2.0, point2.Y + _fractionBar.DesiredSize.Height + 2.0 + DerivativeSymbolVerticalOffset);
        Point point4 = new Point(point.X + _upperDerivativeSymbol.DesiredSize.Width, 0.0);
        _fractionBar.ArrangeMathNode(new Rect(point2, _fractionBar.DesiredSize));
        _upperDerivativeSymbol.ArrangeMathNode(new Rect(point, _upperDerivativeSymbol.DesiredSize));
        _lowerDerivativeSymbol.ArrangeMathNode(new Rect(point3, _lowerDerivativeSymbol.DesiredSize));
        _orderSymbol.ArrangeMathNode(new Rect(point4, _orderSymbol.DesiredSize));
        return DesiredSize;
    }

    public void CopyToOrderSymbol(VisualMathNode orderOperand)
    {
        if (orderOperand is VisualTextNode)
        {
            _orderSymbol = new VisualSymbolNode(((VisualTextNode)orderOperand).Text);
        }
        else if (orderOperand is VisualIdNode)
        {
            _orderSymbol = new VisualSymbolNode(((VisualIdNode)orderOperand).Identifier.ToString());
        }
        else
        {
            _orderSymbol = new VisualSpacer();
        }
        InitializeChildren();
    }

    public override void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        base.HandleMathFormatChanged(e);
        SetOrderSymbolFontSize(MathFormat);
    }
}
