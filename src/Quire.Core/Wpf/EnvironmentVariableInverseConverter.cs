#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnvironmentVariableInverseConverter
{
    [MirrorVerified("PtcWpfEnvironmentVariableInverseConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfEnvironmentVariableInverseConverterMechanicalDifferentialTests")]
    public EnvironmentVariableInverseConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter == null)
        {
            return Visibility.Collapsed;
        }
        return DevelopmentEnvironmentState.RtNameToWrapperFuncMap[(string)parameter]() ? Visibility.Collapsed : Visibility.Visible;
    }
}
