using System;
using System.Windows.Media;

namespace Quire.Controls.Plots.Contour;

internal partial struct HsvColor
{
    private float _hue;

    private const float MinHue = 0f;

    private const float MaxHue = 360f;

    private float _saturation;

    private const float MinSaturation = 0f;

    public const float MaxSaturation = 1f;

    private float _value;

    private const float MinValue = 0f;

    private const float MaxValue = 1f;

    private const float StandardGamma = 2.2f;

    public float Hue
    {
        get => _hue;
        set
        {
            _hue = value % 360f;
            if (_hue < 0f)
            {
                _hue += 360f;
            }
        }
    }

    public float Saturation
    {
        get => _saturation;
        set
        {
            global::Quire.DebugTools.DevAssert(0f <= value && value <= 1f, null);
            _saturation = value;
        }
    }

    public float Value
    {
        get => _value;
        set
        {
            global::Quire.DebugTools.DevAssert(0f <= value && value <= 1f, null);
            _value = value;
        }
    }

    public float Chroma => Value * Saturation;

    private float HPrime => Hue / 60f;

    private float X => Chroma * (1f - Math.Abs(HPrime % 2f - 1f));

    public Color Rgb
    {
        get
        {
            float hPrime = HPrime;
            float minRgb = MinRgb;
            if (hPrime < 1f)
            {
                return Color.FromScRgb(1f, minRgb + Chroma, minRgb + X, minRgb);
            }
            if (hPrime < 2f)
            {
                return Color.FromScRgb(1f, minRgb + Chroma, minRgb + X, minRgb);
            }
            if (hPrime < 3f)
            {
                return Color.FromScRgb(1f, minRgb, minRgb + Chroma, minRgb + X);
            }
            if (hPrime < 4f)
            {
                return Color.FromScRgb(1f, minRgb, minRgb + X, minRgb + Chroma);
            }
            if (hPrime < 5f)
            {
                return Color.FromScRgb(1f, minRgb + X, minRgb, minRgb + Chroma);
            }
            if (hPrime < 6f)
            {
                return Color.FromScRgb(1f, minRgb + Chroma, minRgb, minRgb + X);
            }
            return Color.FromScRgb(1f, minRgb, minRgb, minRgb);
        }
    }

    private float MinRgb => Value - Chroma;

    public float Gamma
    {
        set => Value = (float)Math.Pow(value, 2.200000047683716);
    }

    public static HsvColor FromHsv(float hue, float saturation, float value)
    {
        return new HsvColor
        {
            _hue = hue,
            _saturation = saturation,
            _value = value
        };
    }
}
