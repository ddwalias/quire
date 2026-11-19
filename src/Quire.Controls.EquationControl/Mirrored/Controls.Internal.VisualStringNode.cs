using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

internal partial class VisualStringNode
{
    private double _baseline;
    private double _exponentBaseline;
    protected FormattedText _formattedText;
    private IList<double> _characterWidths;

    public override double TextBaselineY => _baseline;

    public override string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Rect HighlightRect()
    {
        Rect result = ComputeAlignmentBox();
        result.Inflate(1.0, 1.0);
        return result;
    }

    protected static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualStringNode)d).FireTextChanged(e);
    }

    protected void FireTextChanged(DependencyPropertyChangedEventArgs e)
    {
        UpdateText();
        if (TextChanged != null)
        {
            TextChanged((string)e.NewValue);
        }
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        _alignmentPoint = new Point(0.0, FormattedNodeText.Baseline / 2.0);
        return new Size(_formattedText.WidthIncludingTrailingWhitespace, _formattedText.Height);
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        dc.DrawText(_formattedText, new Point(0.0, 0.0));
    }

    private Rect ComputeAlignmentBox()
    {
        return new Rect(0.0, 0.0, _formattedText.WidthIncludingTrailingWhitespace, _formattedText.Height);
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
    }

    public override double EffectiveBaselineY => _baseline;

    public override double EffectiveScriptBaselineY => _exponentBaseline;

    public override FormattedText FormattedNodeText
    {
        get
        {
            if (_formattedText == null)
            {
                UpdateText();
            }

            return _formattedText;
        }
    }

    public override IList<double> CharacterWidths
    {
        get
        {
            UpdateText();
            return _characterWidths;
        }
    }

    public override INodeInfo NodeInfo
    {
        get => base.NodeInfo;
        set
        {
            base.NodeInfo = value;
            ValueNodeInfo valueInfo = value as ValueNodeInfo;
            if (valueInfo == null)
            {
                ((VisualTextNodeForEditor)this).Text = string.Empty;
                return;
            }

            ((VisualTextNodeForEditor)this).Text = valueInfo.Value.ToString();
        }
    }

    public Geometry Geometry => ((VisualTextNodeForEditor)this).FormattedNodeText.BuildGeometry(new Point(0.0, 0.0));

    public override IList<double> ScreenCharacterWidths
    {
        get
        {
            IList<double> widths = ((VisualTextNodeForEditor)this).CharacterWidths;
            Point origin = PointToScreen(new Point(0.0, 0.0));
            for (int i = 0; i < widths.Count; i++)
            {
                Point charPoint = new Point(widths[i], 0.0);
                widths[i] = PointToScreen(charPoint).X - origin.X;
            }

            return widths;
        }
    }

    public VisualStringNode()
    {
    }
}
