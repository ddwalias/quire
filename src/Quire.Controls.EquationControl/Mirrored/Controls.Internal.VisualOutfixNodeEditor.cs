using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.EquationEditor;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualOutfixNodeEditor
{
    protected readonly Fence _leftFence;
    protected readonly Fence _rightFence;
    protected RubberBandHighlighter _highlighter;
    protected Point _firstFenceOffset;

    public Direction NodeSide
    {
        get => (Direction)GetValue(NodeSideProperty);
        set => SetValue(NodeSideProperty, value);
    }

    public EquationEditModeEnum EditMode
    {
        set => SetValue(ScoperModeProperty, value);
    }

    protected double CaretOpacity => (double)GetValue(CaretOpacityProperty);

    private static void OnScoperModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualOutfixNodeEditor)d).UpdateTypeover((EquationEditModeEnum)e.NewValue);
    }

    private static void OnScopeOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualOutfixNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualOutfixNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    private static void OnNodeSidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualOutfixNodeEditor)d).OnNodeSideChanged((Direction)e.NewValue);
    }

    private static void OnScopeColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualOutfixNodeEditor)d).OnScopeColorChanged((System.Drawing.Color)e.NewValue);
    }

    public override void GetFocus()
    {
        base.GetFocus();
        SetParensEditorVisibility(Visibility.Visible);
    }

    public override void Remove()
    {
        EditNode.SetParensVisibility(Visibility.Visible);
        if (_highlighter != null)
        {
            _highlighter.Remove();
        }
        _highlighter = null;
        base.Remove();
    }

    private new void BindToSettings()
    {
        base.BindToSettings();
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, CaretOpacityProperty, "CaretHighlightOpacity");
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, ScopeOpacityProperty, "NearestNeighborOpacity");
    }

    private void UpdateTypeover(EquationEditModeEnum mode)
    {
        if (mode != EquationEditModeEnum.Typeover)
        {
            UpdateParens((Direction)GetValue(NodeSideProperty));
            return;
        }
        RemoveHighlight();
        _highlighter = new RubberBandHighlighter(this, this);
        _highlighter.CornerStyle = HighlightCornerStyle.Rounded;
        BindHighlighterProperties(_highlighter, EquationEditModeEnum.Typeover);
        SetParensEditorVisibility(Visibility.Hidden);
    }

    protected void RemoveHighlight()
    {
        if (_highlighter != null)
        {
            _highlighter.Remove();
        }
    }

    protected Fence GetFence()
    {
        Fence result = null;
        if (NodeSide == Direction.Left)
        {
            result = _leftFence;
        }
        else if (NodeSide == Direction.Right)
        {
            result = _rightFence;
        }
        return result;
    }

    protected override void UpdateCaretBlinking()
    {
        Fence fence = GetFence();
        if (fence != null)
        {
            int period = 0;
            if (Blink)
            {
                period = GetCaretBlinkPeriod();
            }
            WpfUtils.Blink(fence, period);
        }
    }

    protected void SetParensEditorVisibility(Visibility visibility)
    {
        EditNode.SetParensVisibility((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        _leftFence.Visibility = visibility;
        _rightFence.Visibility = visibility;
    }

    public new void SetCaretType(bool isReadWrite)
    {
        if (!isReadWrite)
        {
            WpfUtils.BindToSetting(MathcadSettings.Instance, this, VisualMathNodeEditor<VisualOutfixNode>.CaretColorProperty, "ReadOnlyCaretColor");
        }
        base.SetCaretType(isReadWrite);
    }

    private void OnNodeSideChanged(Direction direction)
    {
        UpdateParens(direction);
    }

    protected override void OnCaretColorChanged(System.Drawing.Color color)
    {
        UpdateParens((Direction)GetValue(NodeSideProperty));
    }

    private void OnCaretOpacityChanged(double d)
    {
        UpdateParens((Direction)GetValue(NodeSideProperty));
    }

    private void OnScopeColorChanged(System.Drawing.Color color)
    {
        UpdateParens((Direction)GetValue(NodeSideProperty));
    }
}
