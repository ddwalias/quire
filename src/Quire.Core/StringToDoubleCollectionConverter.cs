#nullable disable
using System.Globalization;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class StringToDoubleCollectionConverter
{
    [MirrorVerified("PtcStringToDoubleCollectionConverterMechanicalDifferentialTests")]
    public StringToDoubleCollectionConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value as string);

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ConvertBack(value as IEnumerable<double>);
}
