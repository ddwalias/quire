using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Quire.Controls;

public partial class AlgorithmIconConverter
{
    private static readonly BitmapImage CheckMarkIcon = new BitmapImage(
        new Uri("..\\Images\\creo_window_checkmark_set.png", UriKind.RelativeOrAbsolute));

    private static readonly BitmapImage SelectedIcon = new BitmapImage(
        new Uri("..\\Images\\active_list_selection16x16.png", UriKind.RelativeOrAbsolute));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = value?.ToString() ?? string.Empty;
        string option = parameter?.ToString() ?? string.Empty;
        bool isAutoSelect = text.StartsWith(AutoSelect, StringComparison.OrdinalIgnoreCase);
        if (((text.Length == 0) | isAutoSelect) && EquationControlConstants.DefaultAlgorithms.Contains(option))
        {
            return CheckMarkIcon;
        }

        if (isAutoSelect)
        {
            string[] parts = text.Split(new[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && StringComparer.OrdinalIgnoreCase.Equals(parts[1], option))
            {
                return SelectedIcon;
            }
        }

        if (StringComparer.OrdinalIgnoreCase.Equals(text, option))
        {
            return CheckMarkIcon;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
