using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Controls.Core;
using Quire.EquationEditor.View;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualTextNodeEditor
{
    public static DependencyProperty CaretPositionProperty;
    public static DependencyProperty CaretHighlightColorProperty;
    public static DependencyProperty CaretHighlightOpacityProperty;

    private readonly Caret _caret = new Caret();
    private readonly Rectangle _highlightBox = new Rectangle();
    private double _highlightX;
    private Rect _idAlignmentBox;
    private Brush _highlightBrush = Brushes.Bisque;

    public int CaretPosition
    {
        get => (int)GetValue(CaretPositionProperty);
        set => SetValue(CaretPositionProperty, value);
    }

    public string Text
    {
        get => EditNode.Text;
        set => EditNode.Text = value;
    }

    public IList<double> CharacterWidths => ((IVisualTextNode)EditNode).CharacterWidths;

    public Geometry Geometry
    {
        get
        {
            return new RectangleGeometry(_idAlignmentBox);
        }
    }

    static VisualTextNodeEditor()
    {
        CaretPositionProperty = CreateDependencyProperty(0, affectsRender: true, affectsArrange: true, OnCaretPositionChanged, "CaretPosition", typeof(int), typeof(VisualTextNodeEditor));
        CaretHighlightColorProperty = CreateDependencyProperty(System.Drawing.Color.Black, affectsRender: true, affectsArrange: false, OnCaretHighlightColorChanged, "CaretHighlightColor", typeof(System.Drawing.Color), typeof(VisualTextNodeEditor));
        CaretHighlightOpacityProperty = CreateDependencyProperty(1.0, affectsRender: true, affectsArrange: false, OnCaretHighlightOpacityChanged, "CaretHighlightOpacity", typeof(double), typeof(VisualTextNodeEditor));
    }

    private static DependencyProperty CreateDependencyProperty(object defaultValue, bool affectsRender, bool affectsArrange, PropertyChangedCallback callback, string dpName, Type dpType, Type ownerType)
    {
        FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
        metadata.DefaultValue = defaultValue;
        metadata.AffectsArrange = affectsArrange;
        metadata.AffectsRender = affectsRender;
        metadata.PropertyChangedCallback += callback;
        return DependencyProperty.Register(dpName, dpType, ownerType, metadata);
    }

    public VisualTextNodeEditor(VisualMathRegionNode region, VisualMathNode adornedElement)
        : base(adornedElement, region)
    {
        BindProperties();
        HookupEvents();
        UpdateMeasure();
        InitializeCaret();
        InitializeHighlightBox();
    }

    private static void OnCaretPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualTextNodeEditor)d).OnCaretPositionChanged();
    }

    private static void OnCaretHighlightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualTextNodeEditor)d).OnHighlightColorChanged((System.Drawing.Color)e.NewValue);
    }

    private static void OnCaretHighlightOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualTextNodeEditor)d).OnHighlightOpacityChanged((double)e.NewValue);
    }

    private void BindProperties()
    {
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, CaretHighlightColorProperty, "TypeoverColor");
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, CaretHighlightOpacityProperty, "CaretHighlightOpacity");
    }

    private void HookupEvents()
    {
        EditNode.TextChanged += OnTextChanged;
    }

    private void UnhookEvents()
    {
        EditNode.TextChanged -= OnTextChanged;
    }

    public override void Remove()
    {
        UnhookEvents();
        base.Remove();
    }

    private void InitializeCaret()
    {
        AddVisualChild(_caret.Line);
        SetValue(CaretPositionProperty, EditNode.Text.Length);
    }

    private void InitializeHighlightBox()
    {
        _highlightBox.BeginInit();
        _highlightBox.Fill = _highlightBrush;
        _highlightBox.Opacity = 0.4;
        _highlightBox.Width = 0.0;
        _highlightBox.EndInit();
        AddVisualChild(_highlightBox);
    }

    public void SetSelection(int start, int end)
    {
        UpdateMeasure();
        _highlightX = GlyphIndexToX(start);
        _highlightBox.Width = GlyphIndexToX(end) - _highlightX;
    }

    public void UnsetSelection()
    {
        _highlightX = 0.0;
        _highlightBox.Width = 0.0;
    }

    public new void SetCaretType(bool isReadWrite)
    {
        if (isReadWrite)
        {
            _caret.Color = MathcadSettings.Instance.CaretColor;
        }
        else
        {
            _caret.Color = MathcadSettings.Instance.ReadOnlyCaretColor;
        }
    }

    protected override void OnCaretColorChanged(System.Drawing.Color color)
    {
        _caret.Color = color;
    }

    protected override void UpdateCaretBlinking()
    {
        _caret.Blink = Blink;
    }

    private void OnCaretPositionChanged()
    {
        UpdateCaretPosition();
    }

    private void OnTextChanged(string id)
    {
        CaretPosition = 0;
        UpdateMeasure();
    }

    private double GlyphIndexToX(int index)
    {
        double num = 0.0;
        IList<double> characterWidths = EditNode.CharacterWidths;
        for (int i = 0; i < index; i++)
        {
            num += characterWidths[i];
        }
        return num;
    }

    private void UpdateCaretPosition()
    {
        double num = GlyphIndexToX(CaretPosition);
        _caret.Line.BeginInit();
        _caret.Line.X1 = num;
        _caret.Line.X2 = num;
        _caret.Line.Y1 = _idAlignmentBox.Top;
        _caret.Line.Y2 = _idAlignmentBox.Bottom;
        _caret.Line.EndInit();
    }

    private void UpdateMeasure()
    {
        FormattedText formattedNodeText = EditNode.FormattedNodeText;
        if (formattedNodeText.Text.Length > 0)
        {
            _idAlignmentBox = new Rect(0.0, 0.0, formattedNodeText.WidthIncludingTrailingWhitespace, formattedNodeText.Height);
            UpdateCaretPosition();
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _caret.Line.Arrange(new Rect(finalSize));
        _highlightBox.Arrange(new Rect(new Point(_highlightX, 0.0), new Size(_highlightBox.Width, _highlightBox.Height)));
        return base.ArrangeOverride(finalSize);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(_idAlignmentBox.Size));
    }

    protected void OnEditNodeMouseMove(object sender, MouseEventArgs e)
    {
        if (Mouse.Captured == EditNode)
        {
            Point position = e.GetPosition(EditNode);
            FireLeftButtonDrag(position.X, position.Y);
            e.Handled = true;
        }
    }

    protected void OnEditNodeMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            Point position = e.GetPosition(this);
            FireLeftButtonDown(position.X, position.Y);
            e.Handled = true;
        }
    }

    protected void OnEditNodeMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        Mouse.Capture(null);
    }

    private void OnHighlightColorChanged(System.Drawing.Color color)
    {
        _highlightBrush = WpfUtils.ChangeBrushColor(_highlightBrush, color);
    }

    private void OnHighlightOpacityChanged(double opacity)
    {
        _highlightBrush.Opacity = opacity;
    }
}
