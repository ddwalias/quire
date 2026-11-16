using System;
using System.Windows.Media;
using Quire.Wpf;

namespace Quire.Controls.Plots.Model;

public partial class AlphaConstColorMapper : IMapper
{
    private const double MinAlphaPortion = 0.3;

    private const double GammaCorrection = 0.45;

    private static readonly uint _invalidTraceColor = WpfUtils.WpfColorToRgb(Colors.Gray);

    private Color _color = Colors.Blue;

    private uint _alpha = 76u;

    private const int AlphaByteMaxValue = 255;

    private const int AlphaByteOffsetInBits = 24;

    public StyleType StyleType => StyleType.AlphaConstColor;

    public Color Color
    {
        get
        {
            return _color;
        }
        set
        {
            _color = value;
        }
    }

    public uint Alpha
    {
        get
        {
            return _alpha;
        }
        set
        {
            _alpha = value;
        }
    }

    public bool UseTraceInvalidStyle { get; set; }

    public AlphaConstColorMapper()
    {
        UseTraceInvalidStyle = false;
    }

    public uint[] MapDensityToRGBA(double[] density, double maxDensityValue)
    {
        uint num = (UseTraceInvalidStyle ? _invalidTraceColor : WpfUtils.WpfColorToRgb(Color));
        int num2 = density.Length;
        uint[] array = new uint[num2];
        for (int i = 0; i < num2; i++)
        {
            if (density[i] > 0.0)
            {
                uint num3 = (uint)(Math.Pow(density[i] / maxDensityValue, 0.45) * (double)(255 - Alpha) + (double)Alpha);
                array[i] = num | (num3 << 24);
            }
        }
        return array;
    }
}
