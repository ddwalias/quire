#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Quire.Controls.Plots;

public partial class TraceLineWeightConverter
{
    public static readonly TraceLineWeightConverter Instance = new TraceLineWeightConverter();

    public const double MinimalWeight = 0.5;

    private TraceLineWeightConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = values[0] is BitmapSource;
        double num = (double)values[1];
        return flag ? MinimalWeight : num;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
