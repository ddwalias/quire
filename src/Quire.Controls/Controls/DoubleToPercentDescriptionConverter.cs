#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls;

public partial class DoubleToPercentDescriptionConverter
{
    [MirrorVerified("PtcControlsDoubleToPercentDescriptionConverterMechanicalDifferentialTests")]
    public DoubleToPercentDescriptionConverter() { }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double num = (double)value;
        return string.Format("{0:0}%", num * 100.0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = value as string;
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }
        double num = 0.01;
        if (text[text.Length - 1] == '%')
        {
            text = text.Substring(0, text.Length - 1);
        }
        if (double.TryParse(text, out var result))
        {
            return result * num;
        }
        return null;
    }
}
