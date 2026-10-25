#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core.Protection;

public partial class SystemIconConverter
{
    public static SystemIconConverter Instance = new SystemIconConverter();

    public object Convert(object value, Type type, object parameter, CultureInfo culture)
    {
        try
        {
            return ImageUtils.GetImageSourceFromIcon(ImageUtils.MessageBoxImageToIcon((MessageBoxImage)parameter));
        }
        catch (Exception)
        {
            return null;
        }
    }

    [MirrorVerified("PtcControlsCoreProtectionSystemIconConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture) => throw new NotSupportedException();

    [MirrorVerified("PtcControlsCoreProtectionSystemIconConverterMechanicalDifferentialTests")]
    public SystemIconConverter() { }
}
