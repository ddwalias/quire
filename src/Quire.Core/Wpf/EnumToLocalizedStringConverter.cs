#nullable disable
using System;
using System.Collections;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnumToLocalizedStringConverter
{
    [MirrorVerified("PtcWpfEnumToLocalizedStringConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfEnumToLocalizedStringConverterMechanicalDifferentialTests")]
    public EnumToLocalizedStringConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is IDictionary dictionary && value != null && dictionary.Contains(value))
        {
            return dictionary[value] ?? value.ToString();
        }
        return value?.ToString();
    }
}
