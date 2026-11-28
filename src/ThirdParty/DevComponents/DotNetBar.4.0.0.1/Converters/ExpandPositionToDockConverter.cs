#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon.Converters;

public partial class ExpandPositionToDockConverter
{
    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (eExpandPosition)Enum.Parse(typeof(eExpandPosition), value.ToString());

    [MirrorVerified("Release IL")]
    public ExpandPositionToDockConverter() { }
}
