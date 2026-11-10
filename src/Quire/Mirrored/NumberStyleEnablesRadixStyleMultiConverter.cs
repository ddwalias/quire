#nullable disable

using System;
using System.Globalization;
using Quire.Controls.Core;
using Quire.Controls.Plots;

namespace Quire;

public partial class NumberStyleEnablesRadixStyleMultiConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || !(values[1] is string))
        {
            return null;
        }

        bool isPlotControl = values[0] is PlotControl;
        bool isGeneralOrDecimal = values[1].Equals(NumberFormat.NumberStyles.General.ToString()) || values[1].Equals(NumberFormat.NumberStyles.Decimal.ToString());
        return !isPlotControl & isGeneralOrDecimal;
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
