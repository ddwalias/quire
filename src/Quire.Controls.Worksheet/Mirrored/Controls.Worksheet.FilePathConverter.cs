using System;
using System.Globalization;
using System.IO;

namespace Quire.Controls.Worksheet;

public partial class FilePathConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        string text = (string)values[0];
        string text2 = (values.Length != 0) ? ((string)values[1]) : null;
        try
        {
            if (text.IsNotEmpty() && Path.IsPathRooted(text))
            {
                return text;
            }
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(string.Format("{0} : {1}", ex.Message, text));
            text = null;
        }
        return string.Format("...\\{0}", text.IsNotEmpty() ? text : text2);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("it is supposed to be one way binding");
    }

    public FilePathConverter() { }
}
