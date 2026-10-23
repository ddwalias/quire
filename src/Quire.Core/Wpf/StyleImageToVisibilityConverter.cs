#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StyleImageToVisibilityConverter
{
    [MirrorVerified("Release IL")]
    public static StyleImageToVisibilityConverter Instance = new StyleImageToVisibilityConverter();

    [MirrorVerified("PtcWpfStyleImageToVisibilityConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfStyleImageToVisibilityConverterMechanicalDifferentialTests")]
    public StyleImageToVisibilityConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        TextStyle textStyle = new TextStyle((Style)value);
        if ((!textStyle.MarkerStyle.HasValue || textStyle.MarkerStyle == TextMarkerStyle.None) && !textStyle.TextAlignment.HasValue && (!textStyle.TextIndent.HasValue || textStyle.TextIndent == 0.0) && (!textStyle.Margin.HasValue || (textStyle.Margin.HasValue && textStyle.Margin.Equals(new Thickness(0.0, 0.0, 0.0, 0.0)))))
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }
}
