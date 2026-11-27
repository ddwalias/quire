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
                double colorDifference = ColorDifference(item.Color, brush.Color);
                if (num > colorDifference)
                {
                    num = colorDifference;
                    result = item;
                }
            }
        }
        return result;
    }

    private static double ColorDifference(Color colorOne, Color colorTwo)
    {
        double num = colorOne.ScR;
        double greenOne = colorOne.ScG;
        double blueOne = colorOne.ScB;
        double alphaOne = colorOne.ScA;
        double redTwo = colorTwo.ScR;
        double greenTwo = colorTwo.ScG;
        double blueTwo = colorTwo.ScB;
        double alphaTwo = colorTwo.ScA;
        return Math.Sqrt((num - redTwo) * (num - redTwo) + (greenOne - greenTwo) * (greenOne - greenTwo) + (blueOne - blueTwo) * (blueOne - blueTwo) + (alphaOne - alphaTwo) * (alphaOne - alphaTwo)) / 2.0;
    }
}
