#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class WarningTypesToBooleanConverter
{
    private WarningsTypesToShow _targetValue;

    [MirrorVerified("PtcControlsCoreWarningTypesToBooleanConverterMechanicalDifferentialTests")]
    public WarningTypesToBooleanConverter() { }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        _targetValue = (WarningsTypesToShow)value;
        return parameter != null && _targetValue.HasFlag((WarningsTypesToShow)parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((value as bool?).GetValueOrDefault())
        {
            _targetValue |= (WarningsTypesToShow)parameter;
        }
        else
        {
            _targetValue &= ~(WarningsTypesToShow)parameter;
        }
        return _targetValue;
    }
}
