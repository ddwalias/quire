#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class IntegerCompareConverter
{
    [MirrorVerified("PtcWpfIntegerCompareConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfIntegerCompareConverterMechanicalDifferentialTests")]
    private CompareOperators CompareOperator { get; set; }
}
