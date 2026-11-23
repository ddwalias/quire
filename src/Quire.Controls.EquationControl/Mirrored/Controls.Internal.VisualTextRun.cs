using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Services;

namespace Quire.Controls.Internal;

internal partial class VisualTextRun
{
    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            if (_text == value)
            {
                return;
            }

            _text = value;
            UpdateText();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private ExtendedFont _font;
    private string _text;
    protected FormattedText _formattedText;
    private double _baseline;
    private double _superscriptBaseline;
    private readonly IList<double> _characterWidths = new List<double>();

    public MathFormatDobjController MathFormat { get; private set; }

    public double Baseline => _baseline;

    public double SuperscriptBaseline => _superscriptBaseline;

    public IList<double> CharacterWidths => _characterWidths;

    public ExtendedFont Font => _font ?? (_font = AppServiceRegistry.GetService<IFontManager>().GetFont(MathFormat));

    public VisualTextRun()
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

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_formattedText == null)
        {
            return new Size(0.0, 0.0);
        }

        return new Size(_formattedText.WidthIncludingTrailingWhitespace, _formattedText.Height);
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (_formattedText != null)
        {
            dc.DrawText(_formattedText, new Point(0.0, 0.0));
        }

        base.OnRender(dc);
    }

    protected virtual void UpdateText()
    {
        if (string.IsNullOrEmpty(Text) || Font == null)
        {
            _formattedText = null;
            return;
        }

        string text = Text;
        CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
        FlowDirection flowDirection = FlowDirection.LeftToRight;
        Typeface fontTypeface = Font.FontTypeface;
        double fontSize = Font.FontSize;
        Brush foreground = Font.Foreground;
        TextDecorationCollection textDecorations = Font.TextDecorations;
        _formattedText = new FormattedText(text, currentUICulture, flowDirection, fontTypeface, fontSize, foreground);
        _formattedText.SetTextDecorations(textDecorations);
        if (text == string.Empty)
        {
            return;
        }

        _baseline = _formattedText.Baseline;
        _superscriptBaseline = _baseline - ExtendedFont.SafeComputeInkBoundingBox(Font.n_Run).Height;
        _characterWidths.Clear();
        for (int i = 0; i < text.Length; i++)
        {
            _characterWidths.Add(new FormattedText(text.Substring(i, 1), currentUICulture, flowDirection, fontTypeface, fontSize, foreground).WidthIncludingTrailingWhitespace);
        }
    }
}
