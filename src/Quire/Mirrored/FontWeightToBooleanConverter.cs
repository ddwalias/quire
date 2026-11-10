#nullable disable

using System;
using System.Globalization;
using System.Windows;

namespace Quire;

public partial class FontWeightToBooleanConverter
{
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return null;
        }

        bool isChecked = (bool)value;
        string fontWeightName = parameter.ToString();
        if (isChecked)
        {
            switch (fontWeightName)
            {
                case "Black":
                    return FontWeights.Black;
                case "Bold":
                    return FontWeights.Bold;
                case "DemiBold":
                    return FontWeights.DemiBold;
                case "ExtraBlack":
                    return FontWeights.ExtraBlack;
                case "ExtraBold":
                    return FontWeights.ExtraBold;
                case "ExtraLight":
                    return FontWeights.ExtraLight;
                case "Heavy":
                    return FontWeights.Heavy;
                case "Light":
                    return FontWeights.Light;
                case "Medium":
                    return FontWeights.Medium;
                case "Normal":
                    return FontWeights.Normal;
                case "Regular":
                    return FontWeights.Regular;
                case "SemiBold":
                    return FontWeights.SemiBold;
                case "Thin":
                    return FontWeights.Thin;
                case "UltraBlack":
                    return FontWeights.UltraBlack;
                case "UltraBold":
                    return FontWeights.UltraBold;
                case "UltraLight":
                    return FontWeights.UltraLight;
            }
        }

        return FontWeights.Normal;
    }
}
