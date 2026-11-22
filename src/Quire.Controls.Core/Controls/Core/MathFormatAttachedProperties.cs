#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MathFormatAttachedProperties
{
    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static DependencyProperty FontFamilyNameProperty;

    public static readonly DependencyProperty FontSizeProperty;

    public static readonly DependencyProperty BoldProperty;

    public static readonly DependencyProperty ItalicProperty;

    public static readonly DependencyProperty UnderlineProperty;

    public static readonly DependencyProperty FontColorProperty;

    public static readonly DependencyProperty HighlightColorProperty;

    static MathFormatAttachedProperties()
    {
        FontFamilyNameProperty = DependencyProperty.RegisterAttached("FontFamilyName", typeof(string), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(FontConstants.DefaultMathFormat.FontFamilyName, FrameworkPropertyMetadataOptions.Inherits, HandleMathFormatChanged, CoerceFontFamily));
        FontSizeProperty = DependencyProperty.RegisterAttached("FontSize", typeof(double), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(FontConstants.DefaultMathFormat.FontSize, FrameworkPropertyMetadataOptions.Inherits, HandleMathFormatChanged, CoerceFontSize));
        BoldProperty = DependencyProperty.RegisterAttached("Bold", typeof(bool), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(FontConstants.DefaultMathFormat.IsBold, FrameworkPropertyMetadataOptions.Inherits, HandleMathFormatChanged));
        ItalicProperty = DependencyProperty.RegisterAttached("Italic", typeof(bool), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(FontConstants.DefaultMathFormat.IsItalic, FrameworkPropertyMetadataOptions.Inherits, HandleMathFormatChanged));
        UnderlineProperty = DependencyProperty.RegisterAttached("Underline", typeof(bool), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(FontConstants.DefaultMathFormat.IsUnderline, FrameworkPropertyMetadataOptions.Inherits, HandleMathFormatChanged));
        FontColorProperty = DependencyProperty.RegisterAttached("FontColor", typeof(Color), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(FontConstants.DefaultMathFormat.FontColor, FrameworkPropertyMetadataOptions.Inherits, HandleMathFormatChanged));
        HighlightColorProperty = DependencyProperty.RegisterAttached("HighlightColor", typeof(Color), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(FontConstants.DefaultMathFormat.HighlightColor, FrameworkPropertyMetadataOptions.Inherits, HandleMathFormatChanged));
        OverrideLabelColorProperty = DependencyProperty.RegisterAttached("OverrideLabelColor", typeof(bool), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, HandleMathFormatChanged));
        HandleChangesEnabledProperty = DependencyProperty.RegisterAttached("HandleChangesEnabled", typeof(bool), typeof(MathFormatAttachedProperties), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
    }

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static string GetFontFamilyName(DependencyObject depObj) => (string)depObj.GetValue(FontFamilyNameProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetFontFamilyName(DependencyObject depObj, string fontFamilyName) => depObj.SetValue(FontFamilyNameProperty, fontFamilyName);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static double GetFontSize(DependencyObject depObj) => (double)depObj.GetValue(FontSizeProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetFontSize(DependencyObject depObj, double fontSize) => depObj.SetValue(FontSizeProperty, fontSize);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static bool GetBold(DependencyObject depObj) => (bool)depObj.GetValue(BoldProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetBold(DependencyObject depObj, bool bold) => depObj.SetValue(BoldProperty, bold);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static bool GetItalic(DependencyObject depObj) => (bool)depObj.GetValue(ItalicProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetItalic(DependencyObject depObj, bool italic) => depObj.SetValue(ItalicProperty, italic);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static bool GetUnderline(DependencyObject depObj) => (bool)depObj.GetValue(UnderlineProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetUnderline(DependencyObject depObj, bool underline) => depObj.SetValue(UnderlineProperty, underline);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static System.Windows.Media.Color GetFontColor(DependencyObject depObj) => (System.Windows.Media.Color)depObj.GetValue(FontColorProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetFontColor(DependencyObject depObj, System.Windows.Media.Color fontColor) => depObj.SetValue(FontColorProperty, fontColor);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static System.Windows.Media.Color GetHighlightColor(DependencyObject depObj) => (System.Windows.Media.Color)depObj.GetValue(HighlightColorProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetHighlightColor(DependencyObject depObj, System.Windows.Media.Color highlightColor) => depObj.SetValue(HighlightColorProperty, highlightColor);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void CopyAllMathFormatProperties(DependencyObject source, DependencyObject target)
    {
        SetFontFamilyName(target, GetFontFamilyName(source));
        SetFontSize(target, GetFontSize(source));
        SetFontColor(target, GetFontColor(source));
        SetHighlightColor(target, GetHighlightColor(source));
        SetBold(target, GetBold(source));
        SetItalic(target, GetItalic(source));
        SetUnderline(target, GetUnderline(source));
    }

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty OverrideLabelColorProperty;

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static bool GetOverrideLabelColor(DependencyObject obj) => (bool)obj.GetValue(OverrideLabelColorProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetOverrideLabelColor(DependencyObject obj, bool val) => obj.SetValue(OverrideLabelColorProperty, val);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearOverrideLabelColor(DependencyObject obj) => obj.ClearValue(OverrideLabelColorProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty HandleChangesEnabledProperty;

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static bool GetHandleChangesEnabled(DependencyObject obj) => (bool)obj.GetValue(HandleChangesEnabledProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetHandleChangesEnabled(DependencyObject obj, bool value) => obj.SetValue(HandleChangesEnabledProperty, value);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearHandleChangesEnabled(DependencyObject obj) => obj.ClearValue(HandleChangesEnabledProperty);



    private static void HandleMathFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        IMathFormatOwner mathFormatOwner = d as IMathFormatOwner;
        if (mathFormatOwner == null || !GetHandleChangesEnabled(d))
        {
            return;
        }
        mathFormatOwner.HandleMathFormatChanged(e);
    }

    private static object CoerceFontFamily(DependencyObject d, object basevalue)
    {
        string text = basevalue as string;
        if (string.IsNullOrEmpty(text))
        {
            return basevalue;
        }
        // Typed local: Roslyn emits `call` for an object-creation receiver, Release IL requires `callvirt`.
        Typeface typeface = new Typeface(text.Contains(FontConstants.EmbedFontDelimiter) ? new FontFamily(new Uri("pack://application:,,,"), text) : new FontFamily(text), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        typeface.TryGetGlyphTypeface(out var glyphTypeface);
        if (glyphTypeface == null)
        {
            return "Mathcad UniMath Prime";
        }
        return basevalue;
    }

    private static object CoerceFontSize(DependencyObject d, object basevalue)
    {
        return (double)basevalue < FontConstants.MinimumFontSizeInPoints ? FontConstants.MinimumFontSizeInPoints : basevalue;
    }

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public MathFormatAttachedProperties() { }
}
