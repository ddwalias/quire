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

    protected override Size ArrangeOverride(Size finalSize)
    {
        for (int num = 0; num < Children.Count; num++)
        {
            Children[num].Arrange(_layout.ChildItems[num].Rect);
        }

        return DesiredSize;
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
}
