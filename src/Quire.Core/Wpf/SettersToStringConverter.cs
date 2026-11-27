#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class SettersToStringConverter
{
    [MirrorVerified("Release IL")]
    public static SettersToStringConverter Instance = new SettersToStringConverter();

    [MirrorVerified("Release IL")]
    public static readonly string EmbedFontDelimiter = "#";

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Style style)
        {
            List<string> list = new List<string>();
            object obj = TextStyleBase.GetSetterValue(style, TextElement.FontFamilyProperty.Name);
            object fontSize = TextStyleBase.GetSetterValue(style, TextElement.FontSizeProperty.Name);
            object fontWeight = TextStyleBase.GetSetterValue(style, TextElement.FontWeightProperty.Name);
            object fontStyle = TextStyleBase.GetSetterValue(style, TextElement.FontStyleProperty.Name);
            if (obj == null)
            {
                obj = TextStyleBase.GetBasedOnSetterValue(style, TextElement.FontFamilyProperty.Name) ?? "Tahoma";
            }
            else
            {
                string source = ((FontFamily)obj).Source;
                if (source != null && source.Contains(EmbedFontDelimiter))
                {
                    obj = source.Substring(source.IndexOf(EmbedFontDelimiter, StringComparison.Ordinal) + 1);
                }
            }
            if (fontSize == null)
            {
                fontSize = TextStyleBase.GetBasedOnSetterValue(style, TextElement.FontSizeProperty.Name);
            }
            if (fontSize == null)
            {
                fontSize = 11.0;
            }
            else
            {
                fontSize = WpfUtils.DipsToPoints((double)fontSize).Round();
            }
            if (fontWeight == null)
            {
                fontWeight = TextStyleBase.GetBasedOnSetterValue(style, TextElement.FontWeightProperty.Name);
            }
            if (fontStyle == null)
            {
                fontStyle = TextStyleBase.GetBasedOnSetterValue(style, TextElement.FontStyleProperty.Name);
            }
            list.Add(obj.ToString());
            list.Add(fontSize.ToString());
            if (fontWeight != null && fontWeight.ToString() != FontWeights.Normal.ToString())
            {
                list.Add(fontWeight.ToString());
            }
            if (fontStyle != null && fontStyle.ToString() != FontStyles.Normal.ToString())
            {
                list.Add(fontStyle.ToString());
            }
            return string.Join(", ", list);
        }
        return string.Empty;
    }

    [MirrorVerified("PtcWpfSettersToStringConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfSettersToStringConverterMechanicalDifferentialTests")]
    public SettersToStringConverter() { }
}
