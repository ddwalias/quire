#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleanToHiddenVisibilityConverter
{
    [MirrorVerified("PtcWpfBooleanToHiddenVisibilityConverterMechanicalDifferentialTests")]
    public BooleanToHiddenVisibilityConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value as bool? == true) ? Visibility.Visible : Visibility.Hidden;
    }

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Visibility.Visible == value as Visibility?;
    }
}
