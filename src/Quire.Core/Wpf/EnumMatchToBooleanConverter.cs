#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnumMatchToBooleanConverter
{
    [MirrorVerified("PtcWpfEnumMatchToBooleanConverterMechanicalDifferentialTests")]
    public EnumMatchToBooleanConverter() { }

    [MirrorVerified("Release IL")]
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return false;
        }
        string text = value.ToString();
        string expectedValue = parameter.ToString();
        return text.Equals(expectedValue, StringComparison.InvariantCultureIgnoreCase);
    }

    [MirrorVerified("Release IL")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return null;
        }
        bool flag = (bool)value;
        string enumValue = parameter.ToString();
        if (flag)
        {
            return Enum.Parse(targetType, enumValue);
        }
        return null;
    }
}
