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
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Style style)
        {
            List<string> list = new List<string>();
            object obj = TextStyleBase.GetSetterValue(style, TextElement.FontFamilyProperty.Name);
            object obj2 = TextStyleBase.GetSetterValue(style, TextElement.FontSizeProperty.Name);
            object obj3 = TextStyleBase.GetSetterValue(style, TextElement.FontWeightProperty.Name);
            object obj4 = TextStyleBase.GetSetterValue(style, TextElement.FontStyleProperty.Name);
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
            if (obj2 == null)
            {
                obj2 = TextStyleBase.GetBasedOnSetterValue(style, TextElement.FontSizeProperty.Name);
            }
            if (obj2 == null)
            {
                obj2 = 11.0;
            }
            else
            {
                obj2 = WpfUtils.DipsToPoints((double)obj2).Round();
            }
            if (obj3 == null)
            {
                obj3 = TextStyleBase.GetBasedOnSetterValue(style, TextElement.FontWeightProperty.Name);
            }
            if (obj4 == null)
            {
                obj4 = TextStyleBase.GetBasedOnSetterValue(style, TextElement.FontStyleProperty.Name);
            }
            list.Add(obj.ToString());
            list.Add(obj2.ToString());
            if (obj3 != null && obj3.ToString() != FontWeights.Normal.ToString())
            {
                list.Add(obj3.ToString());
            }
            if (obj4 != null && obj4.ToString() != FontStyles.Normal.ToString())
            {
                list.Add(obj4.ToString());
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
