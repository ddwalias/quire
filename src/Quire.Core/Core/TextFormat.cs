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
    public const double DefaultFontSizeInPoints = 11d;

    [MirrorVerified("Release IL")]
    private const double FontSizeAccuracy = 1E-09d;

    [MirrorVerified("Release IL")]
    public const string DefaultTextFormatFontFamiliy = "Tahoma";

    // Keep these dependency property fields in Mathcad order before Initialize.
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
    public static readonly DependencyProperty VariantsProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BackgroundProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ForegroundProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MarginProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TextAlignmentProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TextAlignmentExtProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty NameProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MarkerStyleProperty;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TextIndentProperty;

    // Keep these members before Initialize to preserve Mathcad's generated closure ordinal.
    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public TextStyleData TextStyleData { get; private set; }

    [MirrorVerified("Release IL")]
    public static void SetTextFormat(DependencyProperty property, object value, TextBox control)
    {
        if (property == FontFamilyProperty)
        {
            control.FontFamily = (FontFamily)value;
        }
        else if (property == FontSizeProperty)
        {
            control.FontSize = (double)value;
        }
        else if (property == FontWeightProperty)
        {
            control.FontWeight = (FontWeight)value;
        }
        else if (property == FontStyleProperty)
        {
            control.FontStyle = (FontStyle)value;
        }
        else if (property == TextDecorationsProperty)
        {
            control.TextDecorations = (TextDecorationCollection)value;
        }
        else if (property == BackgroundProperty)
        {
            control.Background = (Brush)value;
        }
        else if (property == ForegroundProperty)
        {
            control.Foreground = (Brush)value;
        }
        else if (property == TextAlignmentProperty)
        {
            control.TextAlignment = (TextAlignment)value;
        }
        else if (property == MarginProperty)
        {
            control.Margin = (Thickness)value;
        }
    }

    [MirrorVerified("Release IL")]
    public TextFormat()
    {
        SetDefaults();
    }

    [MirrorVerified("Release IL")]
    public TextFormat(FlowDocument document) : this()
    {
        FontFamily = document.FontFamily;
        FontSize = document.FontSize;
        FontWeight = document.FontWeight;
        FontStyle = document.FontStyle;
        Background = document.Background ?? Background;
        Foreground = document.Foreground;
        TextAlignment = document.TextAlignment;
        Variants = (FontVariants)((DependencyObject)document).GetValue(VariantsProperty);
    }

    [MirrorVerified("Release IL")]
    public TextFormat(TextBox control) : this()
    {
        FontFamily = control.FontFamily;
        FontSize = control.FontSize;
        FontWeight = control.FontWeight;
        FontStyle = control.FontStyle;
        TextDecorations = control.TextDecorations;
        Background = control.Background ?? Background;
        Foreground = control.Foreground;
        TextAlignment = control.TextAlignment;
        TextAlignmentExt = TextAlignmentExt.None;
        MixedFontFamily = false;
        MixedFontSize = false;
        MixedFontWeight = false;
        MixedFontStyle = false;
        MixedTextDecorations = false;
        MixedForeground = false;
        MixedBackground = false;
        MixedMarkerStyle = false;
        MixedTextAlignment = false;
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
    public TextFormat(TextFormat tf, TextFormat agreesWith) : this(tf)
    {
        if (agreesWith != null)
        {
            SetTextFormatValuesAccordingTo(agreesWith, useDefaultValues: true);
            if (MixedFontFamily || agreesWith.MixedFontFamily || !FontFamily.Equals(agreesWith.FontFamily))
            {
                MixedFontFamily = true;
            }
            if (MixedFontSize || agreesWith.MixedFontSize || Math.Abs(agreesWith.FontSize - FontSize) > 1E-09)
            {
                MixedFontSize = true;
            }
            if (MixedFontWeight || agreesWith.MixedFontWeight || !FontWeight.Equals(agreesWith.FontWeight))
            {
                MixedFontWeight = true;
            }
            if (MixedFontStyle || agreesWith.MixedFontStyle || !FontStyle.Equals(agreesWith.FontStyle))
            {
                MixedFontStyle = true;
            }
            if (MixedTextDecorations || agreesWith.MixedTextDecorations || !((object)TextDecorations).Equals(agreesWith.TextDecorations))
            {
                MixedTextDecorations = true;
            }
            if (MixedForeground || agreesWith.MixedForeground || !AreColorsEqual(Foreground, agreesWith.Foreground))
            {
                MixedForeground = true;
            }
            if (MixedBackground || agreesWith.MixedBackground || !AreColorsEqual(Background, agreesWith.Background, assertOnNull: false))
            {
                MixedBackground = true;
            }
            if (MixedMarkerStyle || agreesWith.MixedMarkerStyle || !MarkerStyle.Equals(agreesWith.MarkerStyle))
            {
                MixedMarkerStyle = true;
            }
            if (MixedTextAlignment || agreesWith.MixedTextAlignment || !TextAlignment.Equals(agreesWith.TextAlignment))
            {
                MixedTextAlignment = true;
            }
            if (MixedStyle || agreesWith.MixedStyle || !StyleKey.Equals(agreesWith.StyleKey))
            {
                MixedStyle = true;
            }
        }
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

    private void Initialize(global::System.Windows.Documents.TextRange textRange, bool onlyFirstRun)
    {
        MarkerStyle = GetMarkerStyleForRange(textRange, out var mixed);
        MixedMarkerStyle = mixed;
        Margin = (textRange.GetPropertyValue(Block.MarginProperty) as Thickness?).GetValueOrDefault();
        if (Margin.Value.Left.IsValid() && !Margin.Value.Left.Equals(0.0) && TextIndent.Equals(0.0))
        {
            TextIndent = Margin.Value.Left;
        }

        IEnumerable<Run> enumerable = global::Quire.Wpf.TextRangeExtension.EnumerateRunsInRange(textRange);
        if (enumerable.IsNullOrEmpty())
        {
            FontFamily = (textRange.GetPropertyValue(TextElement.FontFamilyProperty) as FontFamily) ?? FontFamily;
            FontSize = (textRange.GetPropertyValue(TextElement.FontSizeProperty) as double?) ?? FontSize;
            FontWeight = (textRange.GetPropertyValue(TextElement.FontWeightProperty) as FontWeight?) ?? FontWeight;
            FontStyle = (textRange.GetPropertyValue(TextElement.FontStyleProperty) as FontStyle?) ?? FontStyle;
            Background = (textRange.GetPropertyValue(TextElement.BackgroundProperty) as Brush) ?? Background;
            Foreground = (textRange.GetPropertyValue(TextElement.ForegroundProperty) as Brush) ?? Foreground;
            TextDecorations = (textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection) ?? TextDecorations;
            TextAlignment = (textRange.GetPropertyValue(Block.TextAlignmentProperty) as TextAlignment?) ?? TextAlignment;
            Variants = (textRange.GetPropertyValue(Typography.VariantsProperty) as FontVariants?) ?? Variants;
            return;
        }

        List<Run> list = onlyFirstRun ? enumerable.First().Yield().ToList() : enumerable.ToList();
        TextIndent = (textRange.GetPropertyValue(Paragraph.TextIndentProperty) as double?).GetValueOrDefault();
        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, TextElement.FontFamilyProperty) is FontFamily fontFamily)
        {
            FontFamily = fontFamily;
            MixedFontFamily = false;
        }
        else
        {
            FontFamily = new FontFamily(string.Empty);
            MixedFontFamily = true;
        }

        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, TextElement.FontSizeProperty) is double fontSize)
        {
            FontSize = fontSize;
            MixedFontSize = false;
        }
        else
        {
            MixedFontSize = true;
        }

        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, TextElement.FontWeightProperty) is FontWeight fontWeight)
        {
            FontWeight = fontWeight;
            MixedFontWeight = false;
        }
        else
        {
            MixedFontWeight = true;
        }

        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, TextElement.FontStyleProperty) is FontStyle fontStyle)
        {
            FontStyle = fontStyle;
            MixedFontStyle = false;
        }
        else
        {
            MixedFontStyle = true;
        }

        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, Inline.TextDecorationsProperty, TextDecorationsEquals) is TextDecorationCollection textDecorations)
        {
            TextDecorations = textDecorations;
            MixedTextDecorations = false;
        }
        else
        {
            MixedTextDecorations = true;
        }

        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, Typography.VariantsProperty) is FontVariants variants)
        {
            Variants = variants;
        }

        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, TextElement.BackgroundProperty, (object first, object second) => AreColorsEqual(first as Brush, second as Brush, assertOnNull: false)) is Brush background)
        {
            Background = background;
            MixedBackground = false;
        }
        else if (list.Count <= 1 || list.Select((Run run) => ((DependencyObject)run).GetValue(TextElement.BackgroundProperty)).All((object val) => val == null))
        {
            Background = null;
            MixedBackground = false;
        }
        else
        {
            MixedBackground = true;
        }

        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, TextElement.ForegroundProperty, (object first, object second) => AreColorsEqual(first as Brush, second as Brush)) is Brush foreground)
        {
            Foreground = foreground;
            MixedForeground = false;
        }
        else
        {
            MixedForeground = true;
        }

        if (global::Quire.Wpf.TextRangeExtension.GetFormattingPropertyFromRuns(list, Block.TextAlignmentProperty) is TextAlignment textAlignment)
        {
            TextAlignmentExt enumValue;
            if (TextAlignment != textAlignment)
            {
                TextAlignment = textAlignment;
            }
            else if (EnumHelper.TryParse<TextAlignmentExt>(textAlignment, out enumValue))
            {
                if (TextAlignmentExt != enumValue)
                {
                    TextAlignmentExt = enumValue;
                }
            }
            else
            {
                DebugTools.DevAssert(false, null);
            }

            MixedTextAlignment = false;
        }
        else
        {
            MixedTextAlignment = true;
        }

        string styleKey = TextElementAttachedProperties.GetStyleKey((DependencyObject)list[0]);
        if (styleKey != null)
        {
            StyleKey = list.FirstOrDefault((Run run) => TextElementAttachedProperties.GetStyleKey((DependencyObject)run) != styleKey) == null ? styleKey : null;
        }
    }

    [MirrorVerified("Release IL")]
    private static TextMarkerStyle GetMarkerStyleForRange(System.Windows.Documents.TextRange textRange, out bool mixed)
    {
        mixed = false;
        List list = global::Quire.Wpf.WpfUtils.FindLogicalAncestor<List>(textRange.Start.Parent);
        List list2 = global::Quire.Wpf.WpfUtils.FindLogicalAncestor<List>(textRange.End.Parent);
        if (textRange.End.Parent is FlowDocument flowDocument)
        {
            list2 = flowDocument.Blocks.LastBlock as List;
        }
        if (list?.MarkerStyle != list2?.MarkerStyle)
        {
            mixed = true;
            return TextMarkerStyle.None;
        }
        if (list == null)
        {
            return TextMarkerStyle.None;
        }
        return list.MarkerStyle;
    }

    [MirrorVerified("Release IL")]
    public void UpdateTextFormat(TextFormat tf)
    {
        if (tf == null)
        {
            return;
        }
        StyleKey = tf.StyleKey;
        BasedOnKey = tf.BasedOnKey;
        TextStyleData = tf.TextStyleData;
        Name = tf.Name;
        FontFamily = tf.FontFamily;
        FontSize = tf.FontSize;
        FontWeight = tf.FontWeight;
        FontStyle = tf.FontStyle;
        TextDecorations = tf.TextDecorations?.Clone();
        Variants = tf.Variants;
        Background = tf.Background;
        Foreground = tf.Foreground;
        TextAlignment = tf.TextAlignment;
        TextAlignmentExt = tf.TextAlignmentExt;
        MarkerStyle = tf.MarkerStyle;
        MixedFontFamily = tf.MixedFontFamily;
        MixedFontSize = tf.MixedFontSize;
        MixedFontWeight = tf.MixedFontWeight;
        MixedFontStyle = tf.MixedFontStyle;
        MixedTextDecorations = tf.MixedTextDecorations;
        MixedForeground = tf.MixedForeground;
        MixedBackground = tf.MixedBackground;
        MixedMarkerStyle = tf.MixedMarkerStyle;
        MixedTextAlignment = tf.MixedTextAlignment;
        Margin = tf.Margin;
        TextIndent = tf.TextIndent;
    }

    [MirrorVerified("Release IL")]
    private static void HandleTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextFormat textFormat)
        {
            if (EnumHelper.TryParse<TextAlignmentExt>(e.NewValue, out var enumValue))
            {
                if (textFormat.TextAlignmentExt != enumValue)
                {
                    textFormat.TextAlignmentExt = enumValue;
                }
            }
            else
            {
                DebugTools.DevAssert(false, null);
            }
            if (textFormat.FormatChanged != null)
            {
                textFormat.FormatChanged(e);
            }
        }
    }

    [MirrorVerified("Release IL")]
    private static void HandleTextFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextFormat textFormat = d as TextFormat;
        if (textFormat != null)
        {
            if (e.NewValue != null || e.OldValue != null)
            {
                if (e.NewValue == null || !e.NewValue.Equals(e.OldValue))
                {
                    goto Changed;
                }
            }
        }
        return;
    Changed:
        if (e.Property == FontSizeProperty)
        {
            textFormat.FirePropertyChanged("FontSizeInPoints");
        }
        textFormat.FormatChanged?.Invoke(e);
    }

    [MirrorVerified("Release IL")]
    private static object HandleBrushCoerceValue(DependencyObject d, object basevalue)
    {
        if (basevalue is Brush brush && !((Freezable)brush).IsFrozen)
        {
            return ((Freezable)brush).GetCurrentValueAsFrozen();
        }
        return basevalue;
    }

    [MirrorVerified("Release IL")]
    public static TextFormat GetTextFormatForWholeDocument(FlowDocument document)
    {
        TextFormat textFormat = new TextFormat(new System.Windows.Documents.TextRange(document.ContentStart, document.ContentEnd));
        TextFormat documentTextFormat = new TextFormat(document);
        if (documentTextFormat.Background == null || !AreColorsEqual(textFormat.Background, documentTextFormat.Background, assertOnNull: false))
        {
            textFormat.Background = Brushes.Transparent;
            textFormat.MixedBackground = true;
        }
        return textFormat;
    }

    [MirrorVerified("Release IL")]
    object global::System.ICloneable.Clone() => Clone();

    [MirrorVerified("Release IL")]
    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

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
    public void UpdateTextStyleData(TextStyleData textStyleData)
    {
        TextStyleData = textStyleData;
        StyleKey = textStyleData.StyleKey;
        BasedOnKey = textStyleData.BaseOnKey;
        Name = textStyleData.StyleName;
        UpdateTextFormat(textStyleData.Style);
    }

    [MirrorVerified("Release IL")]
    public void UpdateAccordingTo(TextFormat tf)
    {
        if (tf != null && this != tf)
        {
            if (!tf.TextAlignment.Equals(TextAlignment))
            {
                TextAlignment = tf.TextAlignment;
            }
            SetTextFormatValuesAccordingTo(tf, useDefaultValues: false);
            if (!tf.MixedFontFamily.Equals(MixedFontFamily))
            {
                MixedFontFamily = tf.MixedFontFamily;
            }
            if (!tf.MixedFontSize.Equals(MixedFontSize))
            {
                MixedFontSize = tf.MixedFontSize;
            }
            if (!tf.MixedFontWeight.Equals(MixedFontWeight))
            {
                MixedFontWeight = tf.MixedFontWeight;
            }
            if (!tf.MixedFontStyle.Equals(MixedFontStyle))
            {
                MixedFontStyle = tf.MixedFontStyle;
            }
            if (!tf.MixedTextDecorations.Equals(MixedTextDecorations))
            {
                MixedTextDecorations = tf.MixedTextDecorations;
            }
            if (!tf.MixedForeground.Equals(MixedForeground))
            {
                MixedForeground = tf.MixedForeground;
            }
            if (!tf.MixedBackground.Equals(MixedBackground))
            {
                MixedBackground = tf.MixedBackground;
            }
            if (!tf.MixedMarkerStyle.Equals(MixedMarkerStyle))
            {
                MixedMarkerStyle = tf.MixedMarkerStyle;
            }
            if (!tf.MixedTextAlignment.Equals(MixedTextAlignment))
            {
                MixedTextAlignment = tf.MixedTextAlignment;
            }
            if (!tf.MixedStyle.Equals(MixedStyle))
            {
                MixedStyle = tf.MixedStyle;
            }
            if (!tf.FontSizeInPoints.Equals(FontSizeInPoints))
            {
                FontSizeInPoints = tf.FontSizeInPoints;
            }
        }
    }

    [MirrorVerified("Release IL")]
    private void SetTextFormatValuesAccordingTo(TextFormat tf, bool useDefaultValues)
    {
        if (!tf.FontFamily.Equals(FontFamily))
        {
            FontFamily = useDefaultValues ? new FontFamily(string.Empty) : tf.FontFamily;
        }
        if (!tf.FontWeight.Equals(FontWeight))
        {
            FontWeight = useDefaultValues ? FontWeights.Normal : tf.FontWeight;
        }
        if (!tf.FontStyle.Equals(FontStyle))
        {
            FontStyle = useDefaultValues ? FontStyles.Normal : tf.FontStyle;
        }
        if (TextDecorations != null && !EqualsAllValues(tf.TextDecorations, TextDecorations))
        {
            TextDecorations = useDefaultValues ? new TextDecorationCollection() : tf.TextDecorations;
        }
        if (!tf.Variants.Equals(Variants))
        {
            Variants = useDefaultValues ? FontVariants.Normal : tf.Variants;
        }
        if (Background != null && !AreColorsEqual(tf.Background, Background, assertOnNull: false))
        {
            Background = useDefaultValues ? Brushes.Transparent : tf.Background;
        }
        if (!AreColorsEqual(tf.Foreground, Foreground))
        {
            Foreground = useDefaultValues ? Brushes.Black : tf.Foreground;
        }
        if (!tf.MarkerStyle.Equals(MarkerStyle))
        {
            MarkerStyle = useDefaultValues ? TextMarkerStyle.None : tf.MarkerStyle;
        }
        if (tf.StyleKey == null || !tf.StyleKey.Equals(StyleKey))
        {
            StyleKey = "";
        }
        if (!tf.TextAlignmentExt.Equals(TextAlignmentExt))
        {
            TextAlignmentExt = useDefaultValues ? TextAlignmentExt.None : tf.TextAlignmentExt;
        }
    }

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
    public TextAlignmentExt TextAlignmentExt { get => (TextAlignmentExt)base.GetValue(TextAlignmentExtProperty); set => SetValueSafely(TextAlignmentExtProperty, value); }

    [MirrorVerified("Release IL")]
    public FontFamily FontFamily { get => (FontFamily)base.GetValue(FontFamilyProperty); set => SetValueSafely(FontFamilyProperty, value); }

    [MirrorVerified("Release IL")]
    public double FontSize { get => (double)base.GetValue(FontSizeProperty); set => SetValueSafely(FontSizeProperty, value); }

    [MirrorVerified("Release IL")]
    public double FontSizeInPoints
    {
        get
        {
            if (MixedFontSize)
            {
                return double.NaN;
            }
            return Math.Round(global::Quire.Wpf.WpfUtils.DipsToPoints(FontSize), 1);
        }
        set
        {
            if (!double.IsNaN(value) && FontSize != global::Quire.Wpf.WpfUtils.PointsToDips(value))
            {
                FontSize = global::Quire.Wpf.WpfUtils.PointsToDips(value);
            }
        }
    }

    [MirrorVerified("Release IL")]
    public string StyleKey
    {
        get
        {
            string text = (string)base.GetValue(StyleKeyProperty);
            if (text != null)
            {
                return text;
            }
            return (string)base.GetValue(NameProperty);
        }
        set => SetValueSafely(StyleKeyProperty, value);
    }

    [MirrorVerified("Release IL")]
    public bool MixedFontSize
    {
        get => _mixedFontSize;
        set
        {
            if (value != _mixedFontSize)
            {
                _mixedFontSize = value;
                FirePropertyChanged("FontSizeInPoints");
            }
        }
    }

    [MirrorVerified("Release IL")]
    public Nullable<Thickness> Margin { get => new Thickness?((Thickness)base.GetValue(MarginProperty)); set => SetValueSafely(MarginProperty, value); }

    [MirrorVerified("Release IL")]
    public FontWeight FontWeight { get => (FontWeight)base.GetValue(FontWeightProperty); set => SetValueSafely(FontWeightProperty, value); }

    [MirrorVerified("Release IL")]
    public string Name { get => (string)base.GetValue(NameProperty); set => SetValueSafely(NameProperty, value); }

    [MirrorVerified("Release IL")]
    public FontStyle FontStyle { get => (FontStyle)base.GetValue(FontStyleProperty); set => SetValueSafely(FontStyleProperty, value); }

    [MirrorVerified("Release IL")]
    public TextDecorationCollection TextDecorations { get => (TextDecorationCollection)base.GetValue(TextDecorationsProperty); set => SetValueSafely(TextDecorationsProperty, value); }

    [MirrorVerified("Release IL")]
    public FontVariants Variants { get => (FontVariants)base.GetValue(VariantsProperty); set => SetValueSafely(VariantsProperty, value); }

    [MirrorVerified("Release IL")]
    public double TextIndent { get => (double)base.GetValue(TextIndentProperty); set => SetValueSafely(TextIndentProperty, value); }

    [MirrorVerified("Release IL")]
    public Brush Foreground { get => (Brush)base.GetValue(ForegroundProperty); set => SetValueSafely(ForegroundProperty, value); }

    public static partial class PropertyNames
    {
        [MirrorVerified("Release IL")]
        public const string FontSizeInPoints = "FontSizeInPoints";
    }
}
