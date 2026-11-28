#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon.Converters;

public partial class ImagePositionToDockConverter
{
    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (eButtonImagePosition)Enum.Parse(typeof(eButtonImagePosition), value.ToString());

    [MirrorVerified("Release IL")]
    public ImagePositionToDockConverter() { }
}
