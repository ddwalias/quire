#nullable disable
#pragma warning disable

using System.Linq;
using Quire.Controls.Core;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots.Presenter;

public partial class TickScale
{
    public double MagnitudeThreashold { get; set; }

    public bool UseMagnitudeThreashold { get; set; }

    public bool Show { get; set; }

    public int Count { get; set; }

    public string[] Labels { get; set; }

    public double[] Values { get; set; }

    public double StartGap { get; set; }

    public double[] Pattern { get; set; }

    public bool[] VisabilityPattern { get; set; }

    public TickScale()
    {
        MagnitudeThreashold = 2.0;
        UseMagnitudeThreashold = true;
    }

    public bool ShouldDrawTick(int tickNumber)
    {
        if (VisabilityPattern != null)
        {
            return VisabilityPattern[tickNumber % VisabilityPattern.Length];
        }
        return true;
    }

    public double TickMarkPosition(int tickmark)
    {
        if (Pattern != null)
        {
            int num = Pattern.Length;
            int num2 = tickmark / num;
            return StartGap + Pattern.Sum() * (double)num2 + Pattern.Take(tickmark % num).Sum();
        }
        return tickmark;
    }

    public void FormatValues(NumberFormat format)
    {
        TickmarkFormatter tickmarkFormatter = new TickmarkFormatter(format);
        double[] valuesWithCorrectedZeroForGeneralFormat = GetValuesWithCorrectedZeroForGeneralFormat(format);
        Labels = new string[valuesWithCorrectedZeroForGeneralFormat.Length];
        for (int i = 0; i < valuesWithCorrectedZeroForGeneralFormat.Length; i++)
        {
            Labels[i] = tickmarkFormatter.FormatReal(valuesWithCorrectedZeroForGeneralFormat[i]);
        }
    }

    private double[] GetValuesWithCorrectedZeroForGeneralFormat(NumberFormat format)
    {
        if (format.NumberStyle != NumberFormat.NumberStyles.General || !UseMagnitudeThreashold)
        {
            return Values;
        }
        int[] array = new int[Values.Length];
        for (int i = 0; i < Values.Length; i++)
        {
            array[i] = GetValueMagnitude(Values[i]);
        }
        double[] array2 = new double[Values.Length];
        for (int j = 0; j < Values.Length; j++)
        {
            // The right-grouped && chains match the short-circuit layout of Mathcad's Release IL.
            bool flag = j != 0 && (Values[j - 1] < 0.0 && (double)(array[j - 1] - array[j]) > MagnitudeThreashold);
            if ((j != Values.Length - 1 && (Values[j + 1] > 0.0 && (double)(array[j + 1] - array[j]) > MagnitudeThreashold)) | flag)
            {
                array2[j] = 0.0;
            }
            else
            {
                array2[j] = Values[j];
            }
        }
        return array2;
    }

    private static int GetValueMagnitude(double value)
    {
        RealNumberFormatter.GetBaseVal(value, 1, out var exponent);
        return exponent;
    }
}
