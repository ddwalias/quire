#nullable disable
using System.IO;
using System.Globalization;
using System.Globalization;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class FilePathToNameConverter
{
    [MirrorVerified("PtcWpfFilePathToNameConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfFilePathToNameConverterMechanicalDifferentialTests")]
    public FilePathToNameConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value as string, parameter as string);

    [MirrorVerified("Release IL")]
    public static string Convert(string filePath, string fallbackName = null)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return fallbackName;
        }
        return Path.GetFileNameWithoutExtension(filePath);
    }
}
