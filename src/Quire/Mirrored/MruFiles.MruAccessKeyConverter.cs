#nullable disable

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Quire.MruFiles;

public partial class MruAccessKeyConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        string fullPathAndFileName = (string)values[0];
        int num = (int)values[1];
        string displayedName = GetDisplayedName(fullPathAndFileName);
        if (num + 1 <= 9)
        {
            // Release IL formats the number with string.Format and then concatenates.
            return string.Format("_{0}. ", num + 1) + displayedName;
        }

        return displayedName;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private string GetDisplayedName(string fullPathAndFileName)
    {
        string fileName = Path.GetFileName(fullPathAndFileName);
        string result = fileName;
        if (fileName.Length > 27)
        {
            result = "..." + fileName.Substring(fileName.Length - 27, 27);
        }

        return result;
    }
}
