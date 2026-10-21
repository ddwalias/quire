#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class AngleMultiConverter
{
    [MirrorVerified("PtcWpfAngleMultiConverterMechanicalDifferentialTests")]
    private AngleMultiConverter() { }

    [MirrorVerified("PtcWpfAngleMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("Release IL")]
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is double x && values[1] is double y)
        {
            return Math.Atan2(y, x) * 180.0 / Math.PI;
        }
        DebugTools.DevAssert(values[0] == DependencyProperty.UnsetValue && values[1] == DependencyProperty.UnsetValue, values[0]?.GetType().Name + ", " + values[1]?.GetType().Name);
        return 0;
    }
}
