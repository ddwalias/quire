using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class VisualPlaceholderNodeEditor
{
    public static DependencyProperty DirectionProperty;

    static VisualPlaceholderNodeEditor()
    {
        FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
        metadata.AffectsArrange = true;
        metadata.DefaultValue = Direction.Indeterminate;
        DirectionProperty = DependencyProperty.Register("Direction", typeof(Direction), typeof(VisualPlaceholderNodeEditor), metadata);
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
}
