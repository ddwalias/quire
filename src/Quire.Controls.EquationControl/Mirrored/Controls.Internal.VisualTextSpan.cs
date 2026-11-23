using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Quire.Controls.Core;
using Quire.Core;
using Quire.Services;

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



    protected virtual void UpdateChildren() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public event PropertyChangedEventHandler PropertyChanged;
}
