#nullable disable
using System.Globalization;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class GeneralTransformToTransformConverter
{
    [MirrorVerified("Release IL")]
    public static GeneralTransformToTransformConverter Instance = new GeneralTransformToTransformConverter();

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Transform result)
        {
            return result;
        }
        return Transform.Identity;
    }

    [MirrorVerified("PtcWpfGeneralTransformToTransformConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfGeneralTransformToTransformConverterMechanicalDifferentialTests")]
    public GeneralTransformToTransformConverter() { }
}
