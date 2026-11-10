using System.Windows;
using System.Windows.Shapes;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualPlaceholderNodeEditor
{
    public static DependencyProperty DirectionProperty;
    private RubberBandHighlighter _highlighter;
    private Caret _caret = new Caret();

    static VisualPlaceholderNodeEditor()
    {
        FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
        metadata.AffectsArrange = true;
        metadata.DefaultValue = Direction.Indeterminate;
        DirectionProperty = DependencyProperty.Register("Direction", typeof(Direction), typeof(VisualPlaceholderNodeEditor), metadata);
    }

    public VisualPlaceholderNodeEditor(VisualMathNode node, VisualMathRegionNode region)
        : base(node, region)
    {
        EditNode.Visibility = Visibility.Hidden;
        _highlighter = new RubberBandHighlighter(region.RootNode as UIElement, node);
        _highlighter.CornerStyle = HighlightCornerStyle.Squared;
        WpfUtils.BindToSetting(MathcadSettings.Instance, _highlighter, RubberBandHighlighter.ColorProperty, "ScopeColor");
        WpfUtils.BindToSetting(MathcadSettings.Instance, _highlighter, UIElement.OpacityProperty, "ScopeOpacity");
        AddVisualChild(_caret.Line);
    }

    public Direction Direction
    {
        get => (Direction)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    public override void Remove()
    {
        EditNode.Visibility = EditNode.Parent is IVisualBinaryOperatorNodeWithCustomVisibility parent ? parent.CustomOperatorVisibility : Visibility.Visible;
        _highlighter.Remove();
        base.Remove();
    }

    protected override void OnCaretColorChanged(System.Drawing.Color color)
    {
        _caret.Color = color;
    }

    protected override void UpdateCaretBlinking()
    {
        _caret.Blink = Blink;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Size result = base.MeasureOverride(constraint);
        _caret.Line.Measure(constraint);
        return result;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Line line = _caret.Line;
        line.BeginInit();
        line.X1 = Direction == Direction.Left ? 0.0 : finalSize.Width;
        line.X2 = line.X1;
        line.Y1 = 0.0;
        line.Y2 = finalSize.Height;
        line.EndInit();
        finalSize.Width += 1.0;
        line.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
        return base.ArrangeOverride(finalSize);
    }
}
