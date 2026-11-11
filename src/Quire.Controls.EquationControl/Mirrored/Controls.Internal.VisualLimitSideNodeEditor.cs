using System;
using System.Windows;
using System.Windows.Media;
using Quire.Wpf;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualLimitSideNodeEditor : VisualMathNodeEditor<VisualLimitSideNode>
{
    public static readonly DependencyProperty CaretOpacityProperty;

    private RubberBandHighlighter _highlighter;
    private Caret _caret = new Caret();
    private VisualSymbolNode _limitSideSymbol;

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualLimitSideNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    static VisualLimitSideNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.AffectsRender = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualLimitSideNodeEditor), frameworkPropertyMetadata);
    }

    protected override void UpdateCaretBlinking()
    {
        int period = 0;
        if (Blink)
        {
            period = GetCaretBlinkPeriod();
        }
        if (_limitSideSymbol != null)
        {
            WpfUtils.Blink(_limitSideSymbol, period);
        }
        else
        {
            WpfUtils.Blink(_caret.Line, period);
        }
    }

    protected override void OnCaretColorChanged(Color color)
    {
        UpdateSymbols();
    }

    private void OnCaretOpacityChanged(double d)
    {
        UpdateSymbols();
    }

    private void UpdateSymbols()
    {
        if (_limitSideSymbol != null)
        {
            System.Windows.Media.Color value = WpfUtils.GdiToWpf(GetCaretColor());
            _limitSideSymbol.MathFormat.FontColor = value;
            _limitSideSymbol.Opacity = (double)GetValue(CaretOpacityProperty);
        }
    }

    private new void BindToSettings()
    {
        base.BindToSettings();
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, CaretOpacityProperty, "CaretHighlightOpacity");
    }

    public VisualLimitSideNodeEditor(VisualMathNode node, VisualMathRegionNode region)
        : base(node, region)
    {
        EditNode.Visibility = Visibility.Hidden;
        if (EditNode.IsPlaceholder)
        {
            _highlighter = new RubberBandHighlighter(region.RootNode as UIElement, node);
            _highlighter.CornerStyle = HighlightCornerStyle.Squared;
            WpfUtils.BindToSetting(MathcadSettings.Instance, _highlighter, RubberBandHighlighter.ColorProperty, "ScopeColor");
            WpfUtils.BindToSetting(MathcadSettings.Instance, _highlighter, UIElement.OpacityProperty, "ScopeOpacity");
            AddVisualChild(_caret.Line);
        }
        else
        {
            _limitSideSymbol = ((VisualSymbolNode)EditNode.Child).Clone();
            AddVisualChild(_limitSideSymbol);
        }
        BindToSettings();
    }

    public override void Remove()
    {
        if (EditNode.IsPlaceholder)
        {
            _highlighter.Remove();
        }
        EditNode.Visibility = Visibility.Visible;
        base.Remove();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (EditNode.IsPlaceholder)
        {
            _caret.Line.Measure(constraint);
        }
        else
        {
            _limitSideSymbol.Measure(constraint);
        }
        return base.MeasureOverride(constraint);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (EditNode.IsPlaceholder)
        {
            _caret.Line.BeginInit();
            _caret.Line.X1 = finalSize.Width;
            _caret.Line.X2 = _caret.Line.X1;
            _caret.Line.Y1 = 0.0;
            _caret.Line.Y2 = finalSize.Height;
            _caret.Line.EndInit();
            finalSize.Width += 1.0;
            _caret.Line.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
        }
        else
        {
            _limitSideSymbol.Arrange(new Rect(finalSize));
        }
        return base.ArrangeOverride(finalSize);
    }
}
