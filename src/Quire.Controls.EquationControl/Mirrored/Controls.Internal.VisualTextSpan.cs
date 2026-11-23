using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Quire.Controls.Core;
using Quire.Core;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualTextSpan
{
    private InlineCollection _inlines;
    private double _baseline;
    private double _superscriptBaseline;
    private double _subscriptBaseline;
    private ExtendedFont _font;
    private ScriptedTextLayoutManager _layout;
    private static IFontManager _fontManager;
    private readonly IList<double> _characterWidths = new List<double>();

    private static IFontManager FontManager
    {
        get
        {
            if (_fontManager == null || !_fontManager.IsConnected)
            {
                return _fontManager = AppServiceRegistry.GetService<IFontManager>();
            }

            return _fontManager;
        }
    }

    public InlineCollection Inlines
    {
        get => _inlines;
        set
        {
            if (_inlines == value)
            {
                return;
            }

            _inlines = value;
            UpdateChildren();
        }
    }

    public MathFormatDobjController MathFormat { get; private set; }

    public ExtendedFont Font => _font ?? (_font = FontManager.GetFont(MathFormat));

    public double Baseline => _baseline;

    public double SuperscriptBaseline => _superscriptBaseline;

    public double SubscriptBaseline => _subscriptBaseline;

    public IList<double> CharacterWidths => _characterWidths;

    public VisualTextSpan()
    {
        MathFormat = new MathFormatDobjController(this);
    }

    public void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        if (DebugTools.DevAssert(_font == null, null).Passed)
        {
            return;
        }

        if (e.Property != MathFormatAttachedProperties.HighlightColorProperty)
        {
            _font = null;
        }
    }

    public ExtendedFont GetSubscriptFont()
    {
        return FontManager.GetScaledFont(MathFormat, FontConstants.SubscriptIDFontSizeRatio);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
        {
            return new Size(0.0, 0.0);
        }

        for (int i = 0; i < Children.Count; i++)
        {
            UIElement child = Children[i];
            child.Measure(availableSize);
            double textBaselineOffset = 0.0;
            if (child is IVisualText visualText)
            {
                textBaselineOffset = visualText.Baseline;
            }

            _layout.ChildItems[i].Size = child.DesiredSize;
            _layout.ChildItems[i].TextBaselineOffset = textBaselineOffset;
        }

        _layout.Arrange();
        _baseline = _layout.TextBaselineOffset;
        _superscriptBaseline = _baseline - ExtendedFont.SafeComputeInkBoundingBox(Font.n_Run).Height;
        _subscriptBaseline = _layout.SubscriptBaseline;
        _characterWidths.Clear();
        foreach (IVisualText child in Children)
        {
            child.CharacterWidths.Each(cwidth => _characterWidths.Add(cwidth));
        }

        return _layout.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        for (int num = 0; num < Children.Count; num++)
        {
            Children[num].Arrange(_layout.ChildItems[num].Rect);
        }

        return DesiredSize;
    }



    protected virtual void UpdateChildren()
    {
        Children.Clear();
        _baseline = 0.0;
        _superscriptBaseline = 0.0;
        _characterWidths.Clear();
        if (Font == null || Inlines == null || Inlines.Count == 0)
        {
            return;
        }

        List<TextLayoutItem> list = new List<TextLayoutItem>();
        foreach (Inline inline in Inlines)
        {
            if (inline is Run run)
            {
                VisualTextRun visualTextRun = new VisualTextRun();
                Children.Add(visualTextRun);
                visualTextRun.Text = run.Text;
                ExtendedFont font = visualTextRun.Font;
                list.Add(new TextLayoutItem
                {
                    ItemType = TextLayoutItem.ItemTypes.Normal,
                    CapsHeight = font.GlyphTypeface.CapsHeight * font.FontSize,
                    XHeight = font.GlyphTypeface.XHeight * font.FontSize,
                });
                continue;
            }
            // Keep the Span pattern variable at loop-body scope so its local slot precedes the Run block's locals.
            if (inline is Span span)
            {
                bool flag = span is Subscript;
                VisualTextSpan visualTextSpan = new VisualTextSpan();
                visualTextSpan.MathFormat.SetPropertiesFrom(MathFormat, true);
                if (flag)
                {
                    visualTextSpan.MathFormat.FontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.SubscriptIDFontSizeRatio);
                }

                Children.Add(visualTextSpan);
                visualTextSpan.Inlines = span.Inlines;
                ExtendedFont font2 = visualTextSpan.Font;
                list.Add(new TextLayoutItem
                {
                    ItemType = flag ? TextLayoutItem.ItemTypes.Subscript : TextLayoutItem.ItemTypes.Normal,
                    CapsHeight = font2.GlyphTypeface.CapsHeight * font2.FontSize,
                    XHeight = font2.GlyphTypeface.XHeight * font2.FontSize,
                });
            }
        }

        ExtendedFont subscriptFont = GetSubscriptFont();
        TextLayoutItem defaultSubscriptItem = new TextLayoutItem
        {
            ItemType = TextLayoutItem.ItemTypes.Subscript,
            TextBaselineOffset = subscriptFont.GlyphTypeface.Baseline * subscriptFont.FontSize,
            CapsHeight = subscriptFont.GlyphTypeface.CapsHeight * subscriptFont.FontSize,
        };
        _layout = new ScriptedTextLayoutManager(list.ToArray(), defaultSubscriptItem);
        foreach (IVisualText child in Children)
        {
            child.CharacterWidths.Each(cwidth => _characterWidths.Add(cwidth));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
