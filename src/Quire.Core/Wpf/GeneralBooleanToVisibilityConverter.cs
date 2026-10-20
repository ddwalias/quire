#nullable disable
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class GeneralBooleanToVisibilityConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value != Reverse)
        {
            return Visibility.Visible;
        }
        return Collapse ? Visibility.Collapsed : Visibility.Hidden;
    }

    [MirrorVerified("PtcWpfGeneralBooleanToVisibilityConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfGeneralBooleanToVisibilityConverterMechanicalDifferentialTests")]
    public GeneralBooleanToVisibilityConverter() { }

    [MirrorVerified("PtcWpfGeneralBooleanToVisibilityConverterMechanicalDifferentialTests")]
    public bool Collapse { get; set; }

    [MirrorVerified("PtcWpfGeneralBooleanToVisibilityConverterMechanicalDifferentialTests")]
    public bool Reverse { get; set; }
}
