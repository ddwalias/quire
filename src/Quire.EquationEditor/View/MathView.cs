using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.Tree;

namespace Quire.EquationEditor.View;

/// <summary>Draws a statement (with its result) or a single expression in 2D.</summary>
public class MathView : FrameworkElement
{
    public static readonly DependencyProperty StatementProperty = Register(nameof(Statement), typeof(Statement));
    public static readonly DependencyProperty ExpressionProperty = Register(nameof(Expression), typeof(MathNode));
    public static readonly DependencyProperty ResultProperty = Register(nameof(Result), typeof(MathNode));
    public static readonly DependencyProperty ResultIsTableProperty = Register(nameof(ResultIsTable), typeof(bool), false);
    public static readonly DependencyProperty ErrorSymbolProperty = Register(nameof(ErrorSymbol), typeof(string));
    public static readonly DependencyProperty MathStyleProperty = Register(nameof(MathStyle), typeof(MathStyle), MathStyle.Default);

    private static DependencyProperty Register(string name, Type type, object? defaultValue = null) =>
        DependencyProperty.Register(name, type, typeof(MathView),
            new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                (d, _) => ((MathView)d)._box = null));

    private Box? _box;

    public Statement? Statement { get => (Statement?)GetValue(StatementProperty); set => SetValue(StatementProperty, value); }
    public MathNode? Expression { get => (MathNode?)GetValue(ExpressionProperty); set => SetValue(ExpressionProperty, value); }
    public MathNode? Result { get => (MathNode?)GetValue(ResultProperty); set => SetValue(ResultProperty, value); }
    public bool ResultIsTable { get => (bool)GetValue(ResultIsTableProperty); set => SetValue(ResultIsTableProperty, value); }
    public string? ErrorSymbol { get => (string?)GetValue(ErrorSymbolProperty); set => SetValue(ErrorSymbolProperty, value); }
    public MathStyle MathStyle { get => (MathStyle)GetValue(MathStyleProperty); set => SetValue(MathStyleProperty, value); }

    /// <summary>Padding around the drawing.</summary>
    public Thickness Padding { get; set; } = new(2);

    private Box? GetBox()
    {
        if (_box != null) return _box;
        var style = MathStyle with { PixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip };
        var builder = new VisualNodeBuilder(style, ErrorSymbol);
        if (Statement != null) _box = builder.BuildStatement(Statement, Result, ResultIsTable);
        else if (Expression != null) _box = builder.BuildExpression(Expression);
        return _box;
    }

    /// <summary>Distance from the top of the control to the main baseline.</summary>
    public double BaselineOffset => Padding.Top + (GetBox()?.Ascent ?? 0);

    protected override Size MeasureOverride(Size availableSize)
    {
        var box = GetBox();
        if (box == null) return new Size(Padding.Left + Padding.Right + 8, Padding.Top + Padding.Bottom + MathStyle.FontSize);
        return new Size(box.Width + Padding.Left + Padding.Right, box.Height + Padding.Top + Padding.Bottom);
    }

    protected override void OnRender(DrawingContext dc)
    {
        dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        GetBox()?.Draw(dc, Padding.Left, BaselineOffset);
    }
}
