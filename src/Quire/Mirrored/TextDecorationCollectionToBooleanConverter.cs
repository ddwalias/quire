#nullable disable

using System;
using System.Globalization;
using System.Windows;

namespace Quire;

public partial class TextDecorationCollectionToBooleanConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return false;
        }

        string text = "";
        if (value is TextDecorationCollection { Count: 1 } textDecorationCollection)
        {
            if (object.Equals(textDecorationCollection[0].Location, TextDecorations.Baseline[0].Location))
            {
                text = "Baseline";
            }
            else if (object.Equals(textDecorationCollection[0].Location, TextDecorations.OverLine[0].Location))
            {
                text = "OverLine";
            }
            else if (object.Equals(textDecorationCollection[0].Location, TextDecorations.Strikethrough[0].Location))
            {
                text = "Strikethrough";
            }
            else if (object.Equals(textDecorationCollection[0].Location, TextDecorations.Underline[0].Location))
            {
                text = "Underline";
            }
        }

        string value2 = parameter.ToString();
        return text.Equals(value2, StringComparison.InvariantCultureIgnoreCase);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return null;
        }

        bool isChecked = (bool)value;
        string decorationName = parameter.ToString();
        if (isChecked)
        {
            switch (decorationName)
            {
                case "Baseline":
                    return TextDecorations.Baseline;
                case "OverLine":
                    return TextDecorations.OverLine;
                case "Strikethrough":
                    return TextDecorations.Strikethrough;
                case "Underline":
                    return TextDecorations.Underline;
            }
        }

        return new TextDecorationCollection();
    }
}
