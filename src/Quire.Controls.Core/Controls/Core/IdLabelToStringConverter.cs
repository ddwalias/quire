#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class IdLabelToStringConverter
{
    [MirrorVerified("PtcControlsCoreIdLabelToStringConverterMechanicalDifferentialTests")]
    public IdLabelToStringConverter() { }

    public static string Convert(object value, bool? noneNotNull)
    {
        if (value == null)
        {
            return null;
        }
        if (value.Equals(IdLabels.None))
        {
            if (!noneNotNull.GetValueOrDefault())
            {
                return null;
            }
            return "*";
        }
        return value.ToString().ToUpper();
    }

    public object Convert(object value, Type targetType, object noneNotNull, CultureInfo culture)
    {
        return Convert(value, (bool?)noneNotNull);
    }

    public static IdLabels? ConvertBack(string value, bool? noneNotNull)
    {
        string text;
        if (value == null)
        {
            return null;
        }
        text = value.ToUpper();
        switch (text)
        {
            case "VARIABLE":
                return IdLabels.Variable;
            case "CONSTANT":
                return IdLabels.Constant;
            case "FUNCTION":
                return IdLabels.Function;
            case "UNIT":
                return IdLabels.Unit;
            case "SYSTEM":
                return IdLabels.System;
            case "KEYWORD":
                return IdLabels.Keyword;
            case "NONE":
            case "*":
                if (noneNotNull.GetValueOrDefault())
                {
                    return IdLabels.None;
                }
                break;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object noneNotNull, CultureInfo culture)
    {
        return ConvertBack((string)value, (bool?)noneNotNull);
    }
}
