#nullable disable
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class IsNotNullOrEmptyToVisibilityConverter
{
    static IsNotNullOrEmptyToVisibilityConverter()
    {
        InstanceUsingHidden = new IsNotNullOrEmptyToVisibilityConverter(false);
        InstanceUsingCollapsed = new IsNotNullOrEmptyToVisibilityConverter(true);
    }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null || value.Equals(string.Empty)
            ? (_collapsed ? Visibility.Collapsed : Visibility.Hidden)
            : Visibility.Visible;
    }

    [MirrorVerified("PtcWpfIsNotNullOrEmptyToVisibilityConverterMechanicalDifferentialTests")]
    private readonly bool _collapsed;

    [MirrorVerified("PtcWpfIsNotNullOrEmptyToVisibilityConverterMechanicalDifferentialTests")]
    private IsNotNullOrEmptyToVisibilityConverter(bool collapsed)
    {
        _collapsed = collapsed;
    }

    [MirrorVerified("PtcWpfIsNotNullOrEmptyToVisibilityConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
