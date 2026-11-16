using System.Globalization;
using System.Text;

namespace Quire.Controls.Plots.Contour.Model;

internal partial struct Levels
{
    private double[] _array;

    private int _number;

    internal const string RlLevelsStringFormat = "(op_matrix (unboxed {0}) (unboxed 1) (list {1}))";

    internal double[] Array
    {
        get
        {
            return _array;
        }
        set
        {
            _array = value;
            _number = _array.Length;
        }
    }

    internal int Number
    {
        get
        {
            return _number;
        }
        set
        {
            _number = value;
            _array = null;
        }
    }

    private string ArrayRl
    {
        get
        {
            if (Array != null)
            {
                return string.Format(CultureInfo.InvariantCulture, "(op_matrix (unboxed {0}) (unboxed 1) (list {1}))", Number, LevelsAsSequenceOfNumbers);
            }
            return string.Empty;
        }
    }

    private string LevelsAsSequenceOfNumbers
    {
        get
        {
            StringBuilder stringBuilder = new StringBuilder();
            double[] array = Array;
            foreach (double num in array)
            {
                stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " (number {0})", num);
            }
            return stringBuilder.ToString();
        }
    }

    internal string RL
    {
        get
        {
            if (Array == null)
            {
                return NumberRl;
            }
            return ArrayRl;
        }
    }

    private string NumberRl
    {
        get
        {
            if (Number <= 0)
            {
                return string.Empty;
            }
            return string.Format(CultureInfo.InvariantCulture, "(number {0})", Number);
        }
    }
}
