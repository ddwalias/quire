#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StyleNameToCategoryConverter
{
    [MirrorVerified("PtcWpfStyleNameToCategoryConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfStyleNameToCategoryConverterMechanicalDifferentialTests")]
    public StyleNameToCategoryConverter() { }

    [MirrorVerified("PtcWpfStyleNameToCategoryConverterMechanicalDifferentialTests")]
    public string CategoryLegacy { get; set; }

    [MirrorVerified("PtcWpfStyleNameToCategoryConverterMechanicalDifferentialTests")]
    public string CategoryStandard { get; set; }
}
