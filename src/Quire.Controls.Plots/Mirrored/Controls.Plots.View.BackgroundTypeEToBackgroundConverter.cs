#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.View;

public partial class BackgroundTypeEToBackgroundConverter
{
    public static BackgroundTypeEToBackgroundConverter Instance = new BackgroundTypeEToBackgroundConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BackgroundTypeE backgroundType)
        {
            // Mathcad returns from each case directly, as with the other switch converters.
            switch (backgroundType)
            {
                case BackgroundTypeE.Paper:
                    return new SolidColorBrush(BackgroundTypeColors.Paper);
                case BackgroundTypeE.White:
                    return Brushes.White;
                case BackgroundTypeE.Transparent:
                    return Brushes.Transparent;
                case BackgroundTypeE.Gray:
                    return new SolidColorBrush(BackgroundTypeColors.Gray);
                // Undefined shares the default block; the IL switch table starts at 0 and needs that entry.
                case BackgroundTypeE.Undefined:
                default:
                    return new SolidColorBrush(BackgroundTypeColors.Paper);
            }
        }
        return new SolidColorBrush(BackgroundTypeColors.Paper);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
