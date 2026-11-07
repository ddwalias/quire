#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RealNumberFormatter
{
    private readonly IFormatterHelper _helper;

    [MirrorVerified("PtcControlsCoreRealNumberFormatterMechanicalDifferentialTests")]
    public NumberFormat DisplayNumberFormat { get; private set; }

    public RealNumberFormatter(NumberFormat numberFormat, IFormatterHelper helper)
    {
        DisplayNumberFormat = numberFormat;
        _helper = helper;
    }

    public string FormatReal(double value)
    {
        if (DisplayNumberFormat.ShouldBeDisplayedAsZero(value))
        {
            value = 0.0;
        }
        switch (DisplayNumberFormat.NumberStyle)
        {
            case NumberFormat.NumberStyles.Scientific:
                return FormatRealToExponential(value, 1);
            case NumberFormat.NumberStyles.Engineering:
                return FormatRealToExponential(value, 3);
            case NumberFormat.NumberStyles.Percent:
                return FormatRealToPercent(value);
            case NumberFormat.NumberStyles.Decimal:
                return FormatRealToDecimal(value);
            default:
                return FormatRealToGeneral(value);
        }
    }

    private string AddNegIfNeeded(double originalValue, string inProcFormated)
    {
        if (!(originalValue < 0.0))
        {
            return inProcFormated;
        }
        return _helper.Neg(inProcFormated);
    }

    private string FormatRealToGeneral(double value)
    {
        if (DisplayNumberFormat.ShouldBeDisplayedAsScientific(value))
        {
            return FormatRealToExponential(value, 1);
        }
        return FormatRealToDecimal(value);
    }

    private string FormatRealToDecimal(double value)
    {
        bool isZero;
        return FormatStringAndIsZeroRepresentation(value, out isZero, str => str);
    }

    private string FormatRealToExponential(double value, int powerBase)
    {
        if (double.IsInfinity(value))
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        int exponent;
        double baseVal = GetBaseVal(value, powerBase, out exponent);
        NumberFormat.ExponentStyles? exponentStyle = DisplayNumberFormat.ExponentStyle;
        if (exponentStyle.HasValue && exponentStyle.GetValueOrDefault() == NumberFormat.ExponentStyles.MultiplyByBaseToAPower)
        {
            return CreateExponentialMultiplyByBaseToAPowerFormat(baseVal, exponent);
        }
        return CreateExponentialENotationFormat(baseVal, exponent);
    }

    public static double GetBaseVal(double value, int powerBase, out int exponent)
    {
        if (value == 0.0)
        {
            exponent = 0;
        }
        else
        {
            exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)) / (double)powerBase) * powerBase;
        }
        return value / Math.Pow(10.0, exponent);
    }

    private string CreateExponentialENotationFormat(double baseVal, int exponent)
    {
        string exp = exponent.ToString("+000;-000", CultureInfo.InvariantCulture);
        bool isZero;
        return FormatStringAndIsZeroRepresentation(baseVal, out isZero, str => str + "E" + exp);
    }

    private double ToStringAndBack(IFormattable d, out string s)
    {
        s = d.ToString(GetFormatStringForDouble(), CultureInfo.InvariantCulture);
        return double.Parse(s, CultureInfo.InvariantCulture);
    }

    private string FormatStringAndIsZeroRepresentation(double doubleValue, out bool isZero, Func<string, string> formatNonZeroValue)
    {
        isZero = 0.0 == ToStringAndBack(Math.Abs(doubleValue), out var s);
        if (!isZero)
        {
            return AddNegIfNeeded(doubleValue, formatNonZeroValue(s));
        }
        return s;
    }

    private string CreateExponentialMultiplyByBaseToAPowerFormat(double baseVal, int exponent)
    {
        bool isZero;
        string text = FormatStringAndIsZeroRepresentation(baseVal, out isZero, str => str);
        if (isZero)
        {
            return text;
        }
        switch (exponent)
        {
            case 0:
                return text;
            case 1:
                return _helper.Mult(text, "10");
            default:
                return _helper.Mult(text, _helper.Exponent("10", FormatExponent(exponent)));
        }
    }

    private string FormatExponent(int exponent)
    {
        if (exponent <= 0)
        {
            return _helper.Neg((-exponent).ToString(CultureInfo.InvariantCulture));
        }
        return exponent.ToString(CultureInfo.InvariantCulture);
    }

    private string FormatRealToPercent(double value)
    {
        bool isZero;
        return FormatStringAndIsZeroRepresentation(value * 100.0, out isZero, str => _helper.Percent(str));
    }

    private string GetFormatStringForDouble()
    {
        string result;
        int num = DisplayNumberFormat.DecimalPlaces ?? (-1);
        if (num == -1)
        {
            result = "R";
        }
        else if (DisplayNumberFormat.ShowTrailingDecimalZeros)
        {
            result = "0." + new string('0', num);
        }
        else
        {
            result = "0." + new string('#', num);
        }
        return result;
    }
}
