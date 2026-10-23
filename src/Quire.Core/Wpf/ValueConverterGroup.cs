#nullable disable
using System.Globalization;
using System.Linq;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ValueConverterGroup
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return this.Aggregate(value, (object current, System.Windows.Data.IValueConverter converter) => converter.Convert(current, targetType, parameter, culture));
    }

    [MirrorVerified("PtcWpfValueConverterGroupMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfValueConverterGroupMechanicalDifferentialTests")]
    public ValueConverterGroup() { }
}
