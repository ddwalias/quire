#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StringsMatchToBooleanConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values[0] == null || values.Length != 2)
        {
            return false;
        }
        return values[0].ToString() == values[1].ToString();
    }

    [MirrorVerified("PtcWpfStringsMatchToBooleanConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfStringsMatchToBooleanConverterMechanicalDifferentialTests")]
    public StringsMatchToBooleanConverter() { }
}
