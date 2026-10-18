#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DummyConverter
{
    [MirrorVerified("PtcWpfDummyConverterMechanicalDifferentialTests")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

    [MirrorVerified("PtcWpfDummyConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;

    [MirrorVerified("PtcWpfDummyConverterMechanicalDifferentialTests")]
    public DummyConverter() { }
}
