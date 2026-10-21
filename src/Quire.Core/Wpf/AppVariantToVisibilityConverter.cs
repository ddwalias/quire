#nullable disable
using System.Globalization;
using System.Windows;
using Quire.Mirror;
using Quire.Services;

namespace Quire.Wpf;

public partial class AppVariantToVisibilityConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (ApplicationVariant.IsEngNbPlus.GetValueOrDefault() ? true : ApplicationVariant.IsEngNb.GetValueOrDefault()) ? Visibility.Collapsed : Visibility.Visible;
    }

    [MirrorVerified("PtcWpfAppVariantToVisibilityConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfAppVariantToVisibilityConverterMechanicalDifferentialTests")]
    public AppVariantToVisibilityConverter() { }
}
