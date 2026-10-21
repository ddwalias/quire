#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class CachedDoubleStringConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double number = (double)value;
        if (doubleCache.Equals(number) && strCache != null)
        {
            return strCache;
        }
        if (number == 0.0)
        {
            return 0;
        }
        if (-0.0001 < number && number < 0.0001)
        {
            return number.ToString("0.###############", CultureInfo.InvariantCulture);
        }
        return number.ToString(CultureInfo.InvariantCulture);
    }

    [MirrorVerified("PtcWpfCachedDoubleStringConverterMechanicalDifferentialTests")]
    public CachedDoubleStringConverter() { }
}
