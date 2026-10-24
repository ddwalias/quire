#nullable disable
using System.Globalization;
using System.Linq;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleanOperatorMultiConverter
{
    [MirrorVerified("PtcWpfBooleanOperatorMultiConverterMechanicalDifferentialTests")]
    private BooleanOperatorMultiConverter() { }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        string text = parameter.ToString().ToLower();
        switch (text)
        {
            case OperatorOr:
                return values.Any((object value) => true.Equals(value));
            case OperatorAnd:
                return values.All((object value) => true.Equals(value));
            default:
                return null;
        }
    }

    [MirrorVerified("PtcWpfBooleanOperatorMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
