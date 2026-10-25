#nullable disable
using System;
using System.Collections;
using System.Windows.Media;

namespace Quire.Controls.Core;

public static partial class ColorPickerHelper
{
    public static SolidColorBrush FindClosestMatchingColorInPalette(SolidColorBrush brush, IEnumerable colorPaletteCollection)
    {
        SolidColorBrush result = null;
        if (brush != null)
        {
            double num = 100.0;
            foreach (SolidColorBrush item in colorPaletteCollection)
            {
                double num2 = ColorDifference(item.Color, brush.Color);
                if (num > num2)
                {
                    num = num2;
                    result = item;
                }
            }
        }
        return result;
    }

    private static double ColorDifference(Color colorOne, Color colorTwo)
    {
        double num = colorOne.ScR;
        double num2 = colorOne.ScG;
        double num3 = colorOne.ScB;
        double num4 = colorOne.ScA;
        double num5 = colorTwo.ScR;
        double num6 = colorTwo.ScG;
        double num7 = colorTwo.ScB;
        double num8 = colorTwo.ScA;
        return Math.Sqrt((num - num5) * (num - num5) + (num2 - num6) * (num2 - num6) + (num3 - num7) * (num3 - num7) + (num4 - num8) * (num4 - num8)) / 2.0;
    }
}
