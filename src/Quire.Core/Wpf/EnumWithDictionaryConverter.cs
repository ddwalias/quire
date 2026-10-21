#nullable disable
using System.Collections;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnumWithDictionaryConverter
{
    [MirrorVerified("Release IL")]
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(parameter is IDictionary dictionary))
        {
            return value.ToString();
        }
        return dictionary[value];
    }

    [MirrorVerified("PtcWpfEnumWithDictionaryConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfEnumWithDictionaryConverterMechanicalDifferentialTests")]
    public EnumWithDictionaryConverter() { }
}
