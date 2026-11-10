#nullable disable

using System;
using System.Globalization;
using Quire.Calculation.Core;

namespace Quire;

public partial class UnitSystemsToStringConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Enum.GetName(typeof(UnitSystemsDataSource), (UnitSystemsDataSource)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value as string)
        {
            case "SI":
                return UnitSystems.SI;
            case "US":
            case "USCS":
                return UnitSystems.US;
            case "CGS":
                return UnitSystems.CGS;
            default:
                return null;
        }
    }
}
