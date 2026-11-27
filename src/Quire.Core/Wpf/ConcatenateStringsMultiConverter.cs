#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ConcatenateStringsMultiConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        string text = string.Empty;
        string separator = parameter.ToString();
        foreach (object obj in values)
        {
            if (text != string.Empty)
            {
                text += separator;
            }
            text += obj.ToString();
        }
        return text;
    }

    [MirrorVerified("Release IL")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new Exception("The method or operation is not implemented.");
    }
}
