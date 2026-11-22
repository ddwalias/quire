using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

internal partial class VisualSymbolNode
{
    protected readonly string _symbol;
    private FormattedText _text;
    private Geometry _geometry;
    protected bool _automaticallyAlign = true;
    private double _baseline;
    private double _glyphGap;
    protected Rect _inkBounds;

    public static readonly DependencyProperty IsMonoLineProperty = DependencyProperty.RegisterAttached("IsMonoLine", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata(false));

    protected SymbolMargin HorizontalMargin
    {
        get
        {
            return new SymbolMargin
            {
                Left = LeftMargin,
                Right = RightMargin
            };
        }
        set
        {
            DebugTools.DevAssert(Margin.Top == 0.0 && Margin.Bottom == 0.0, null);
            Margin = new Thickness(value.Left, 0.0, value.Right, 0.0);
        }
    }

    protected double LeftMargin => Margin.Left;

    protected double RightMargin => Margin.Right;

    public override double TextBaselineY => _baseline;

    public override Rect ScreenInkBounds
    {
        get
        {
            Rect inkBounds = _inkBounds;
            inkBounds.Offset(0.0, _baseline);
            Point topLeft = PointToScreen(inkBounds.TopLeft);
            Point bottomRight = PointToScreen(inkBounds.BottomRight);
            return new Rect(topLeft, bottomRight);
        }
    }

    public double GlyphGap => Math.Abs(_glyphGap);

    public override double EffectiveBaselineY => 0.0;

    public override double EffectiveScriptBaselineY => 0.0;

    public virtual Geometry Geometry
    {
        get
        {
            if (_geometry == null)
            {
                _geometry = _text.BuildGeometry(new Point(GetXCoordinate(), 0.0));
            }
            return _geometry;
        }
    }

    internal Rect SymbolBounds
    {
        get
        {
            UpdateText(Font);
            return new Rect(GetXCoordinate(), 0.0, _text.Width, _text.Height);
        }
    }

    public static bool GetIsMonoLine(DependencyObject dObj)
    {
        return (bool)dObj.GetValue(IsMonoLineProperty);
    }

    public static void SetIsMonoLine(DependencyObject dObj, bool value)
    {
        dObj.SetValue(IsMonoLineProperty, value);
    }

    public VisualSymbolNode(char symbol)
        : this(symbol, 1)
    {
    }

    public VisualSymbolNode(char symbol, SymbolMargin horizMargin)
        : this(symbol.ToString(), horizMargin)
    {
    }

    public VisualSymbolNode(string symbol)
        : this(symbol, 1)
    {
    }

    public VisualSymbolNode(string symbol, SymbolMargin horizMargin)
    {
        _symbol = symbol;
        HorizontalMargin = horizMargin;
        SetOperatorFontProperties(MathFormat);
        ExtendedFont font = FontManager.GetFont(MathFormat);
        UpdateText(font);
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        Binding binding = new Binding
        {
            Source = Parent,
            Path = new PropertyPath(ShowOperatorOnlyWhenActiveProperty)
        };
        SetBinding(ShowOperatorOnlyWhenActiveProperty, binding);
    }

    protected override void OnShowOperatorOnlyWhenActiveChanged(bool value)
    {
        if (!value)
        {
            return;
        }
        MathFormat.FontColor = Colors.DarkGray;
        Binding binding = new Binding
        {
            Source = this,
            Path = new PropertyPath(EquationControl.ActiveElementOpacityProperty)
        };
        SetBinding(UIElement.OpacityProperty, binding);
    }

    protected override void OnRender(DrawingContext dc)
    {
        UpdateText(Font);
        dc.DrawText(_text, new Point(GetXCoordinate(), 0.0));
    }

    private double GetXCoordinate()
    {
        return Math.Min(LeftMargin, 0.0);
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        Rect symbolBounds = SymbolBounds;
        return symbolBounds.Size;
    }

    protected void UpdateText(ExtendedFont font)
    {
        if (_symbol == null)
        {
            return;
        }
        _text = new FormattedText(_symbol.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, font.FontTypeface, font.FontSize, font.Foreground);
        GlyphRun glyphRun = ExtendedFont.GenerateGlyphRun(_text.Text, font.GlyphTypeface.FontUri, font.FontSize);
        Rect alignmentBox = glyphRun.ComputeAlignmentBox();
        _inkBounds = ExtendedFont.SafeComputeInkBoundingBox(glyphRun);
        _glyphGap = alignmentBox.Top - _inkBounds.Top;
        _baseline = glyphRun.BaselineOrigin.Y;
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        return DesiredSize;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        return null;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public VisualSymbolNode Clone()
    {
        VisualSymbolNode visualSymbolNode = new VisualSymbolNode(_symbol, HorizontalMargin);
        visualSymbolNode.MathFormat.SetPropertiesFrom(MathFormat);
        return visualSymbolNode;
    }
}
