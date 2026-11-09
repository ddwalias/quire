using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quire.Controls.ControlAdorners;

public partial class OuterGrabbingAreaMarginConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Thickness thickness = values[0] is Thickness ? (Thickness)values[0] : default(Thickness);
        double width = values[1] is double ? (double)values[1] : 0.0;
        double height = values[2] is double ? (double)values[2] : 0.0;
        double centerX = (width - thickness.Left - thickness.Right) / 2.0 + thickness.Left;
        double centerY = (height - thickness.Top - thickness.Bottom) / 2.0 + thickness.Top;
        return new Thickness(centerX - 20.0, centerY - 20.0, width - centerX - 20.0, height - centerY - 20.0);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public partial class OuterGrabbingAreaVisibilityConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        double width = values[0] is double ? (double)values[0] : 0.0;
        double height = values[1] is double ? (double)values[1] : 0.0;
        if (width < 40.0 || height < 40.0)
        {
            return Visibility.Visible;
        }

        return Visibility.Hidden;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
