using System;
using System.Globalization;
using System.Windows;

namespace Quire.Controls.InputControls;

public partial class HorizontalThicknessSubtractionConverter
{
    public static HorizontalThicknessSubtractionConverter Instance = new HorizontalThicknessSubtractionConverter();

    private HorizontalThicknessSubtractionConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double number && parameter is Thickness thickness)
        {
            return number - thickness.Left - thickness.Right;
        }

        global::Quire.DebugTools.FailAssertion(value?.GetType().Name ?? "", Array.Empty<object>());
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
