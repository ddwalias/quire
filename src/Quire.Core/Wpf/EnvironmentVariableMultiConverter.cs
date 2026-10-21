#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnvironmentVariableMultiConverter
{
    [MirrorVerified("Release IL")]
    public static EnvironmentVariableMultiConverter Instance;

    [MirrorVerified("Release IL")]
    static EnvironmentVariableMultiConverter()
    {
        Instance = new EnvironmentVariableMultiConverter();
    }

    [MirrorVerified("PtcWpfEnvironmentVariableMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfEnvironmentVariableMultiConverterMechanicalDifferentialTests")]
    public EnvironmentVariableMultiConverter() { }
}
