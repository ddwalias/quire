#nullable disable
using System;
using System.Globalization;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DropDownListFontFamilyConverter
{
    [MirrorVerified("Release IL")]
    public static DropDownListFontFamilyConverter Instance = new DropDownListFontFamilyConverter();

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        string text = ((FontFamily)value).Source;
        if (text != null && text.Contains("#"))
        {
            text = text.Substring(text.IndexOf("#", StringComparison.Ordinal) + 1);
        }
        return text;
    }

    [MirrorVerified("PtcWpfDropDownListFontFamilyConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfDropDownListFontFamilyConverterMechanicalDifferentialTests")]
    public DropDownListFontFamilyConverter() { }
}
