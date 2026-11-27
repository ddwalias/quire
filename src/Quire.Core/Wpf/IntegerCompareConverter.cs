#nullable disable
using System.Globalization;
using System.Globalization;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class IntegerCompareConverter
{
    [MirrorVerified("Release IL")]
    private static bool ToInt(object value, out int num)
    {
        bool flag = false;
        num = 0;
        if (value != null)
        {
            if (value is int)
            {
                num = (int)value;
                flag = true;
            }
            else if (value is string && int.TryParse(value.ToString(), out num))
            {
                flag = true;
            }
            if (!flag)
            {
                DebugTools.FailAssertion("Unsupported input type.");
            }
        }
        return flag;
    }

    [MirrorVerified("PtcWpfIntegerCompareConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfIntegerCompareConverterMechanicalDifferentialTests")]
    private CompareOperators CompareOperator { get; set; }

    [MirrorVerified("Release IL")]
    private IntegerCompareConverter(IntegerCompareConverter.CompareOperators compareOperator)
    {
        CompareOperator = compareOperator;
    }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!ToInt(value, out var currentValue) || !ToInt(parameter, out var comparedValue))
        {
            return false;
        }
        switch (CompareOperator)
        {
            case CompareOperators.GreaterThan:
                return currentValue > comparedValue;
            case CompareOperators.GreaterThanOrEquals:
                return currentValue >= comparedValue;
            case CompareOperators.LessThan:
                return currentValue < comparedValue;
            case CompareOperators.LessThanOrEquals:
                return currentValue <= comparedValue;
            case CompareOperators.EqualsTo:
                return currentValue == comparedValue;
            case CompareOperators.NotEqualsTo:
                return currentValue != comparedValue;
            default:
                throw new NotImplementedException();
        }
    }
}
