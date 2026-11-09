using System;
using System.Globalization;
using System.Windows.Data;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.Traces;

namespace Quire.Controls.Plots;

public partial class LineStyleToDashArrayConverter : IValueConverter
{
    public static readonly LineStyleToDashArrayConverter Instance = new LineStyleToDashArrayConverter();

    private LineStyleToDashArrayConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return TraceRenderer._TraceLineDictionary[value is LineStyle ? (LineStyle)value : LineStyle.None].Dashes;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
