#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ValueConverterGroup
{
    [MirrorVerified("PtcWpfValueConverterGroupMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfValueConverterGroupMechanicalDifferentialTests")]
    public ValueConverterGroup() { }
}
