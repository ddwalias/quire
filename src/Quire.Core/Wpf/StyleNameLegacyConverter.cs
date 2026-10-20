#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StyleNameLegacyConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = (string)value;
        if (text != null && text.StartsWith("Legacy"))
        {
            return text.Substring("Legacy".Length);
        }
        return value;
    }

    [MirrorVerified("PtcWpfStyleNameLegacyConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfStyleNameLegacyConverterMechanicalDifferentialTests")]
    public StyleNameLegacyConverter() { }
}
