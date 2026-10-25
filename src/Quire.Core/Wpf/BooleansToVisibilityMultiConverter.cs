#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleansToVisibilityMultiConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length < 2)
        {
            return Collapse ? global::System.Windows.Visibility.Collapsed : global::System.Windows.Visibility.Hidden;
        }

        bool firstIsTrue = values[0] is bool firstValue && firstValue;
        bool secondIsTrue = values[1] is bool secondValue && secondValue;
        if (firstIsTrue || secondIsTrue)
        {
            return global::System.Windows.Visibility.Visible;
        }

        return Collapse ? global::System.Windows.Visibility.Collapsed : global::System.Windows.Visibility.Hidden;
    }

    [MirrorVerified("PtcWpfBooleansToVisibilityMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfBooleansToVisibilityMultiConverterMechanicalDifferentialTests")]
    public BooleansToVisibilityMultiConverter() { }

    [MirrorVerified("PtcWpfBooleansToVisibilityMultiConverterMechanicalDifferentialTests")]
    public bool Collapse { get; set; }
}
