#nullable disable
using System.Globalization;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StyleToImageConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        TextStyle textStyle = new TextStyle((Style)value);
        if (textStyle.MarkerStyle == TextMarkerStyle.Decimal)
        {
            return new BitmapImage(new Uri("pack://application:,,,/MathcadPrime;component/images/text_list_numbered_16x16.png"));
        }
        if (textStyle.MarkerStyle == TextMarkerStyle.Disc)
        {
            return new BitmapImage(new Uri("pack://application:,,,/MathcadPrime;component/images/text_list_bulleted_16x16.png"));
        }
        if ((textStyle.TextIndent.HasValue && textStyle.TextIndent != 0.0) || (textStyle.Margin.HasValue && !textStyle.Margin.Equals(new Thickness(0.0, 0.0, 0.0, 0.0))))
        {
            return new BitmapImage(new Uri("pack://application:,,,/MathcadPrime;component/images/text_indent_increase_16x16.png"));
        }
        if (textStyle.TextAlignment.HasValue)
        {
            switch (textStyle.TextAlignment)
            {
                case TextAlignment.Center:
                    return new BitmapImage(new Uri("pack://application:,,,/MathcadPrime;component/images/text_align_center_16x16.png"));
                case TextAlignment.Justify:
                    return new BitmapImage(new Uri("pack://application:,,,/MathcadPrime;component/images/text_align_justify_16x16.png"));
                case TextAlignment.Right:
                    return new BitmapImage(new Uri("pack://application:,,,/MathcadPrime;component/images/text_align_right_16x16.png"));
                case TextAlignment.Left:
                    return new BitmapImage(new Uri("pack://application:,,,/MathcadPrime;component/images/text_align_left_16x16.png"));
            }
        }
        return null;
    }

    [MirrorVerified("PtcWpfStyleToImageConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfStyleToImageConverterMechanicalDifferentialTests")]
    public StyleToImageConverter() { }
}
