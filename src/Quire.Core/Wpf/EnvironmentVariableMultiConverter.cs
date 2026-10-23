#nullable disable
using System.Globalization;
using System.Linq;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnvironmentVariableMultiConverter
{
    [MirrorVerified("Release IL")]
    public static EnvironmentVariableMultiConverter Instance;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length == 0)
        {
            return Visibility.Collapsed;
        }
        return values.Any((object value) => DevelopmentEnvironmentState.RtNameToWrapperFuncMap[(string)value]()) ? Visibility.Visible : Visibility.Collapsed;
    }

    [MirrorVerified("PtcWpfEnvironmentVariableMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfEnvironmentVariableMultiConverterMechanicalDifferentialTests")]
    public EnvironmentVariableMultiConverter() { }

    [MirrorVerified("Release IL")]
    static EnvironmentVariableMultiConverter()
    {
        Instance = new EnvironmentVariableMultiConverter();
    }
}
