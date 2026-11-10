#nullable disable

using System;
using System.Globalization;
using Quire.Controls.Core;

namespace Quire;

public partial class RadixStyleToStringConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // The named subject local keeps Mathcad's slot order (string, then the object result), and the (object) cast types the arms.
        string radixStyleName = value as string;
        return radixStyleName switch
        {
            "Decimal" => (object)NumberFormat.RadixStyles.Decimal,
            "Binary" => NumberFormat.RadixStyles.Binary,
            "Octal" => NumberFormat.RadixStyles.Octal,
            "Hexadecimal" => NumberFormat.RadixStyles.Hexadecimal,
            _ => null,
        };
    }
}
