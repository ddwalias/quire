#nullable disable
using System.Globalization;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class StringToDoubleCollectionConverter
{
    [MirrorVerified("Release IL")]
    public IEnumerable<double> Convert(string strValue)
    {
        if (strValue == null)
        {
            return null;
        }

        return strValue.Split(Separator).Select((string str) =>
        {
            if (!double.TryParse(str, out var result))
            {
                result = double.NaN;
            }

            return result;
        });
    }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value as string);

    [MirrorVerified("Release IL")]
    public string ConvertBack(IEnumerable<double> arrDoubles)
    {
        if (!DebugTools.AssertNotNull(arrDoubles, null).Failed)
        {
            return string.Join(Separator.ToString(), arrDoubles.Select((double dim) => dim.ToString(CultureInfo.InvariantCulture)));
        }

        return null;
    }

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ConvertBack(value as IEnumerable<double>);

    [MirrorVerified("PtcStringToDoubleCollectionConverterMechanicalDifferentialTests")]
    public StringToDoubleCollectionConverter() { }
}
