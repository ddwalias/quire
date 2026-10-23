#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnumMatchToVisibilityConverter
{
    private static readonly EnumMatchToBooleanConverter EnumMatchToBooleanConverterInstance = new EnumMatchToBooleanConverter();

    [MirrorVerified("Release IL")]
    public static EnumMatchToVisibilityConverter Instance = new EnumMatchToVisibilityConverter();

    [MirrorVerified("PtcWpfEnumMatchToVisibilityConverterMechanicalDifferentialTests")]
    private EnumMatchToVisibilityConverter() { }

    [MirrorVerified("PtcWpfEnumMatchToVisibilityConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("Release IL")]
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = (bool)EnumMatchToBooleanConverterInstance.Convert(value, targetType, parameter, culture);
        return BooleanToVisibilityConverter.Instance.Convert(flag, targetType, null, culture);
    }
}
