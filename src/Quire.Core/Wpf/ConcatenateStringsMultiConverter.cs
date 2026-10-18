#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ConcatenateStringsMultiConverter
{
    [MirrorVerified("PtcWpfConcatenateStringsMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new Exception("The method or operation is not implemented.");

    [MirrorVerified("PtcWpfConcatenateStringsMultiConverterMechanicalDifferentialTests")]
    public ConcatenateStringsMultiConverter() { }
}
