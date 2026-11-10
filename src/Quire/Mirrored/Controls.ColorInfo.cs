#nullable disable

namespace Quire.Controls;

public partial struct ColorInfo
{
    private const double MaxByteValueAsDouble = 255.0;
    private const double MaxDegrees = 360.0;
    private const double Degrees60 = 60.0;
    private const double Degrees120 = 120.0;
    private const double Degrees240 = 240.0;
    private const double MaxPresent = 100.0;
    private const double Tolerance = 1E-15;

    public double Red;
    public double Green;
    public double Blue;
    public double Hue;
    public double Saturation;
    public const byte Alpha = 255;
    public float Lum;

    public ColorInfo(double red, double green, double blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Hue = 0.0;
        Saturation = 0.0;
        Lum = 0f;
        RgbToHsl(red, green, blue, out Hue, out Saturation, out Lum);
    }

    public ColorInfo(string sValue)
    {
        Red = global::System.Convert.ToUInt32(sValue.Substring(3, 2), 16);
        Green = global::System.Convert.ToUInt32(sValue.Substring(5, 2), 16);
        Blue = global::System.Convert.ToUInt32(sValue.Substring(7, 2), 16);
        Hue = 0.0;
        Saturation = 0.0;
        Lum = 0f;
        RgbToHsl(Red, Green, Blue, out Hue, out Saturation, out Lum);
    }

    public ColorInfo(double hue, double saturation, float lum)
    {
        Hue = hue;
        Saturation = saturation;
        Lum = lum;
        Red = 0.0;
        Green = 0.0;
        Blue = 0.0;
        HslToRgb(hue, saturation / 100.0, (double)lum / 100.0, out Red, out Green, out Blue);
    }

    private static void RgbToHsl(double red, double green, double blue, out double hue, out double sat, out float lum)
    {
        red /= 255.0;
        green /= 255.0;
        blue /= 255.0;
        double num = global::System.Math.Max(red, global::System.Math.Max(green, blue));
        double num2 = global::System.Math.Min(red, global::System.Math.Min(green, blue));
        double num3 = num - num2;
        lum = (float)(num + num2) / 2f;
        sat = 0.0;
        if ((double)lum < 1E-15 || num3 < 1E-15)
        {
            sat = 0.0;
        }
        else if (0f < lum && lum <= 0.5f)
        {
            sat = num3 / (num + num2);
        }
        else if ((double)lum > 0.5)
        {
            sat = num3 / (2.0 - (num + num2));
        }
        hue = 0.0;
        if (num3 < 1E-15)
        {
            hue = 0.0;
        }
        else if (num - red < 1E-15)
        {
            hue = 60.0 * (green - blue) / num3;
        }
        else if (num - green < 1E-15)
        {
            hue = 60.0 * (blue - red) / num3 + 120.0;
        }
        else if (num - blue < 1E-15)
        {
            hue = 60.0 * (red - green) / num3 + 240.0;
        }
        if (hue < 0.0)
        {
            hue += 360.0;
        }
        hue = global::System.Math.Max(0.0, global::System.Math.Min(360.0, hue));
        sat = global::System.Math.Max(0.0, global::System.Math.Min(1.0, sat)) * 100.0;
        lum = (float)global::System.Math.Max(0.0, global::System.Math.Min(1.0, lum)) * 100f;
    }

    public override string ToString()
    {
        return "#" + byte.MaxValue.ToString("X2") + ((byte)Red).ToString("X2") + ((byte)Green).ToString("X2") + ((byte)Blue).ToString("X2");
    }

    private static void HslToRgb(double hue, double sat, double lum, out double red, out double green, out double blue)
    {
        hue = global::System.Math.Max(0.0, global::System.Math.Min(360.0, hue));
        sat = global::System.Math.Max(0.0, global::System.Math.Min(1.0, sat));
        lum = global::System.Math.Max(0.0, global::System.Math.Min(1.0, lum));
        double num = ((lum < 0.5) ? (lum * (1.0 + sat)) : (lum + sat - lum * sat));
        double num2 = 2.0 * lum - num;
        double num3 = hue / 360.0;
        // Release IL requires separate array element assignments rather than an array initializer
        double[] array = new double[3];
        array[0] = num3 + 1.0 / 3.0;
        array[1] = num3;
        array[2] = num3 - 1.0 / 3.0;
        for (int i = 0; i < 3; i++)
        {
            if (array[i] < 0.0)
            {
                array[i]++;
            }
            if (array[i] > 1.0)
            {
                array[i]--;
            }
            if (array[i] * 6.0 < 1.0)
            {
                array[i] = num2 + (num - num2) * 6.0 * array[i];
            }
            else if (array[i] * 2.0 < 1.0)
            {
                array[i] = num;
            }
            else if (array[i] * 3.0 < 2.0)
            {
                array[i] = num2 + (num - num2) * (2.0 / 3.0 - array[i]) * 6.0;
            }
            else
            {
                array[i] = num2;
            }
        }
        red = global::System.Math.Max(0.0, global::System.Math.Min(255.0, array[0] * 255.0));
        green = global::System.Math.Max(0.0, global::System.Math.Min(255.0, array[1] * 255.0));
        blue = global::System.Math.Max(0.0, global::System.Math.Min(255.0, array[2] * 255.0));
    }
}
