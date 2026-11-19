#nullable disable

using System;
using System.Globalization;
using Quire.Controls.InputControls;
using Quire.Properties;

namespace Quire;

public partial class PythonDirectoryPathConverter
{
    public PythonDirectoryPathConverter() { }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value.ToString().IsNullOrEmpty() && Settings.Default.PythonDirectoryPath.IsNullOrEmpty() && InputControlEnginePython.PythonDirectoryPath.IsNotEmpty())
        {
            Settings.Default.PythonDirectoryPath = InputControlEnginePython.PythonDirectoryPath;
            return InputControlEnginePython.PythonDirectoryPath;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
