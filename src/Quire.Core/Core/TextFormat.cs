#nullable disable
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Core;

public partial class TextFormat
{
    [MirrorVerified("Release IL")]
    public void SetValueSafely(DependencyProperty prop, object value)
    {
        try
        {
            ((DependencyObject)this).SetValue(prop, value);
        }
        catch (ArgumentException ex)
        {
            DebugTools.DevAssert(false, ex.Message);
        }
    }

    [MirrorVerified("Release IL")]
    public static bool AreColorsEqual(Brush first, Brush second, bool assertOnNull = true)
    {
        if (assertOnNull)
        {
            DebugTools.AssertNotNull(first, null);
            DebugTools.AssertNotNull(second, null);
        }
        if (first == null && second == null)
        {
            return true;
        }
        if (first == null || second == null)
        {
            return false;
        }
        SolidColorBrush solidColorBrush = first as SolidColorBrush;
        SolidColorBrush solidColorBrush2 = second as SolidColorBrush;
        if (DebugTools.AssertNotNull(solidColorBrush, null).Passed && DebugTools.AssertNotNull(solidColorBrush2, null).Passed)
        {
            return solidColorBrush.Color == solidColorBrush2.Color;
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public static bool TextDecorationsEquals(object object1, object object2)
    {
        if (object1 is ICollection<TextDecoration> t && object2 is ICollection<TextDecoration> t2)
        {
            return EqualsAllValues(t, t2);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    internal static bool EqualsAllValues(ICollection<TextDecoration> t1, ICollection<TextDecoration> t2)
    {
        if (t1.Count != t2.Count)
        {
            return false;
        }
        foreach (TextDecoration item in t1)
        {
            if (!t2.Contains(item))
            {
                return false;
            }
        }
        return true;
    }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public TextFormat Clone() => new TextFormat(this);

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public TextStyleData TextStyleData { get; private set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public string BasedOnKey { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedFontFamily { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedFontWeight { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedFontStyle { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedTextDecorations { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedForeground { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedBackground { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedMarkerStyle { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedStyle { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedTextAlignment { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public event Delegate1<DependencyPropertyChangedEventArgs> FormatChanged;

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("Release IL")]
    public TextFormat()
    {
        SetDefaults();
    }

    [MirrorVerified("Release IL")]
    public void SetDefaults()
    {
        FontFamily = new FontFamily("Tahoma");
        FontSizeInPoints = 11.0;
        FontWeight = FontWeights.Normal;
        FontStyle = FontStyles.Normal;
        TextDecorations = new TextDecorationCollection();
        Variants = (FontVariants)0;
        Background = Brushes.Transparent;
        Foreground = Brushes.Black;
        TextAlignment = (TextAlignment)0;
        TextAlignmentExt = TextAlignmentExt.None;
        MarkerStyle = (TextMarkerStyle)0;
        TextIndent = 0.0;
    }

    [MirrorVerified("Release IL")]
    public TextFormat(string styleKey) : this()
    {
        StyleKey = styleKey;
    }

    [MirrorVerified("Release IL")]
    public TextFormat(TextStyleData textStyleData) : this()
    {
        UpdateTextStyleData(textStyleData);
    }

    [MirrorVerified("Release IL")]
    public TextFormat(TextFormat tf) : this()
    {
        UpdateTextFormat(tf);
    }

    [MirrorVerified("Release IL")]
    public TextFormat(TextFormat parenTextFormat, global::System.Windows.Documents.TextRange textRange, bool onlyFirstRun = false) : this(parenTextFormat)
    {
        Initialize(textRange, onlyFirstRun);
    }

    [MirrorVerified("Release IL")]
    public TextFormat(global::System.Windows.Documents.TextRange textRange, bool onlyFirstRun = false) : this()
    {
        Initialize(textRange, onlyFirstRun);
    }

    [MirrorVerified("Release IL")]
    public void UpdateTextStyleData(TextStyleData textStyleData)
    {
        TextStyleData = textStyleData;
        StyleKey = textStyleData.StyleKey;
        BasedOnKey = textStyleData.BaseOnKey;
        Name = textStyleData.StyleName;
        UpdateTextFormat(textStyleData.Style);
    }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty VariantsProperty;

    [MirrorVerified("Release IL")]
    public static void CopyTextFormat(FlowDocument from, FlowDocument to)
    {
        to.FontFamily = from.FontFamily;
        to.FontSize = from.FontSize;
        to.FontWeight = from.FontWeight;
        to.FontStyle = from.FontStyle;
        to.Background = from.Background;
        to.Foreground = from.Foreground;
        to.TextAlignment = from.TextAlignment;
        to.SetValue(VariantsProperty, from.GetValue(VariantsProperty));
    }

    [MirrorVerified("Release IL")]
    public static global::System.Windows.Documents.TextRange TextRange(RichTextBox text) => new global::System.Windows.Documents.TextRange(text.Document.ContentStart, text.Document.ContentEnd);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FontFamilyProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FontSizeProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FontWeightProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FontStyleProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TextDecorationsProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BackgroundProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ForegroundProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MarginProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TextAlignmentProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MarkerStyleProperty;

    [MirrorVerified("Release IL")]
    public void ApplyTextFormat(global::System.Windows.Documents.TextRange textRange)
    {
        ApplyPropertyValueFormat(FontFamilyProperty, FontFamily, textRange);
        ApplyPropertyValueFormat(FontSizeProperty, FontSize, textRange);
        ApplyPropertyValueFormat(FontWeightProperty, FontWeight, textRange);
        ApplyPropertyValueFormat(FontStyleProperty, FontStyle, textRange);
        ApplyPropertyValueFormat(TextDecorationsProperty, TextDecorations, textRange);
        ApplyPropertyValueFormat(VariantsProperty, Variants, textRange);
        ApplyPropertyValueFormat(BackgroundProperty, Background, textRange);
        ApplyPropertyValueFormat(ForegroundProperty, Foreground, textRange);
        ApplyPropertyValueFormat(TextAlignmentProperty, TextAlignment, textRange);
        ApplyPropertyValueFormat(MarkerStyleProperty, MarkerStyle, textRange);
        ApplyPropertyValueFormat(MarginProperty, Margin, textRange);
    }

    [MirrorVerified("Release IL")]
    public TextMarkerStyle MarkerStyle { get => (TextMarkerStyle)base.GetValue(MarkerStyleProperty); set => SetValueSafely(MarkerStyleProperty, value); }

    [MirrorVerified("Release IL")]
    public TextAlignment TextAlignment { get => (TextAlignment)base.GetValue(TextAlignmentProperty); set => SetValueSafely(TextAlignmentProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TextAlignmentExtProperty;

    [MirrorVerified("Release IL")]
    public TextAlignmentExt TextAlignmentExt { get => (TextAlignmentExt)base.GetValue(TextAlignmentExtProperty); set => SetValueSafely(TextAlignmentExtProperty, value); }

    [MirrorVerified("Release IL")]
    public FontFamily FontFamily { get => (FontFamily)base.GetValue(FontFamilyProperty); set => SetValueSafely(FontFamilyProperty, value); }

    [MirrorVerified("Release IL")]
    public double FontSize { get => (double)base.GetValue(FontSizeProperty); set => SetValueSafely(FontSizeProperty, value); }

    [MirrorVerified("Release IL")]
    public Nullable<Thickness> Margin { get => new Thickness?((Thickness)base.GetValue(MarginProperty)); set => SetValueSafely(MarginProperty, value); }

    [MirrorVerified("Release IL")]
    public FontWeight FontWeight { get => (FontWeight)base.GetValue(FontWeightProperty); set => SetValueSafely(FontWeightProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty NameProperty;

    [MirrorVerified("Release IL")]
    public string Name { get => (string)base.GetValue(NameProperty); set => SetValueSafely(NameProperty, value); }

    [MirrorVerified("Release IL")]
    public FontStyle FontStyle { get => (FontStyle)base.GetValue(FontStyleProperty); set => SetValueSafely(FontStyleProperty, value); }

    [MirrorVerified("Release IL")]
    public TextDecorationCollection TextDecorations { get => (TextDecorationCollection)base.GetValue(TextDecorationsProperty); set => SetValueSafely(TextDecorationsProperty, value); }

    [MirrorVerified("Release IL")]
    public FontVariants Variants { get => (FontVariants)base.GetValue(VariantsProperty); set => SetValueSafely(VariantsProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TextIndentProperty;

    [MirrorVerified("Release IL")]
    public double TextIndent { get => (double)base.GetValue(TextIndentProperty); set => SetValueSafely(TextIndentProperty, value); }

    [MirrorVerified("Release IL")]
    public Brush Foreground { get => (Brush)base.GetValue(ForegroundProperty); set => SetValueSafely(ForegroundProperty, value); }

    [MirrorVerified("Release IL")]
    public Brush Background { get => (Brush)base.GetValue(BackgroundProperty); set => SetValueSafely(BackgroundProperty, value); }
}
