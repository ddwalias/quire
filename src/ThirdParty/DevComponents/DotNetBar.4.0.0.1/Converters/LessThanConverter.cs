#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon.Converters;

public partial class LessThanConverter
{
    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new NotSupportedException("ConvertBack not supported by this converter.");

    [MirrorVerified("Release IL")]
    public LessThanConverter() { }
}
