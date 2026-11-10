#nullable disable

using System;
using System.Globalization;

namespace Quire;

public partial class EnumMatchToBooleanConverterWithNull
{
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Type underlyingType = Nullable.GetUnderlyingType(targetType);
        if ((bool)value)
        {
            return Enum.Parse(underlyingType, parameter.ToString());
        }

        return null;
    }
}
