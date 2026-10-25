#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class FindOptionToBooleanConverter
{
    private static readonly FindOptions _allSet;

    private FindOptions _targetValue;

    static FindOptionToBooleanConverter()
    {
        _allSet = FindOptions.None;
        foreach (FindOptions value in Enum.GetValues(typeof(FindOptions)))
        {
            _allSet |= value;
        }
    }

    [MirrorVerified("PtcControlsCoreFindOptionToBooleanConverterMechanicalDifferentialTests")]
    public FindOptionToBooleanConverter() { }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        _targetValue = (FindOptions)value;
        return ((FindOptions)parameter & _targetValue) != 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((value as bool?).GetValueOrDefault())
        {
            _targetValue |= (FindOptions)parameter;
        }
        else
        {
            _targetValue &= _allSet ^ (FindOptions)parameter;
        }
        return _targetValue;
    }
}
