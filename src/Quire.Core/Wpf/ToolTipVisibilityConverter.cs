#nullable disable
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ToolTipVisibilityConverter
{
    [MirrorVerified("Release IL")]
    public static ToolTipVisibilityConverter Instance = new ToolTipVisibilityConverter();

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
    }

    [MirrorVerified("PtcWpfToolTipVisibilityConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfToolTipVisibilityConverterMechanicalDifferentialTests")]
    public ToolTipVisibilityConverter() { }
}
