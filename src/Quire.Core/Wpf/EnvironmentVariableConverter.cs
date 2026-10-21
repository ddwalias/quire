#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnvironmentVariableConverter
{
    [MirrorVerified("PtcWpfEnvironmentVariableConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfEnvironmentVariableConverterMechanicalDifferentialTests")]
    public EnvironmentVariableConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter == null)
        {
            return Visibility.Collapsed;
        }
        return DevelopmentEnvironmentState.RtNameToWrapperFuncMap[(string)parameter]() ? Visibility.Visible : Visibility.Collapsed;
    }
}
