using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Quire.EquationEditor.Tree;

namespace Quire.Controls;

public partial class AlgorithmOptionsIconConverter
{
    private static readonly BitmapImage CheckMarkIcon = new BitmapImage(new Uri("..\\Images\\creo_window_checkmark_set.png", UriKind.RelativeOrAbsolute));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is AlgorithmOptions algorithmOptions) || !(parameter is string text))
        {
            return null;
        }

        text = text.Trim();
        bool flag;
        switch (text)
        {
            case "MultiStart":
                flag = algorithmOptions.IsMultiStart;
                break;
            case "Optimize":
                flag = algorithmOptions.IsOptimize;
                break;
            default:
                flag = false;
                break;
        }

        if (!flag)
        {
            return null;
        }

        return CheckMarkIcon;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }

    public AlgorithmOptionsIconConverter()
    {
    }
}
