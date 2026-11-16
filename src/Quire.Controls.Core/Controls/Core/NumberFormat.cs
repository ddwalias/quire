#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NumberFormat
{
    public const int DefaultComplexThresholdValue = 10;
    public const int MinComplexThresholdAllowed = 0;
    public const int MaxComplexThresholdAllowed = 307;
    public const int DefaultZeroThresholdValue = 15;
    public const int MinZeroThresholdAllowed = 0;
    public const int MaxZeroThresholdAllowed = 307;
    public const int DefaultExponentialThresholdValue = 3;
    public const int MinExponentialThresholdAllowed = 0;
    public const int MaxExponentialThresholdAllowed = 15;
    public const int MinDecimalPlacesAllowed = 0;
    public const int MaxDecimalPlacesAllowed = 15;
    public const int DefaultDecimalPlacesValue = 3;
    public const int RoundTripDecimalPlaces = -1;
    public const int DecimalBase = 10;
    public const int BinaryBase = 2;
    public const int OctalBase = 8;
    public const int HexadecimalBase = 16;

    public static readonly RoutedCommand SetNumberFormatFieldCommand = new RoutedUICommand("Set number format field", "SetNumberFormatField", typeof(NumberFormat));
    public static RoutedCommand SetNumberFormatExponentialThresholdCommand;
    public static Control ExponentialThresholdControl;
    public static RoutedCommand SetNumberFormatZeroThresholdCommand;
    public static Control ZeroThresholdControl;
    public static RoutedCommand SetNumberFormatComplexThresholdCommand;
    public static Control ComplexThresholdControl;

    private Nullable<int> _decimalPlaces;
    private bool _showTrailingDecimalZeros;
    private Nullable<NumberStyles> _numberStyle;
    private Nullable<ExponentStyles> _exponentStyle;
    private Nullable<ComplexStyles> _complexStyle;
    private Nullable<int> _zeroThreshold;
    private Nullable<int> _exponentialThreshold;
    private Nullable<int> _complexThreshold;
    private Nullable<RadixStyles> _radixStyle;

    public Nullable<int> DecimalPlaces
    {
        get => _decimalPlaces;
        set
        {
            if (value == _decimalPlaces || DebugTools.DevAssert(IsValidDecimalPlaces(value), null).Failed)
            {
                return;
            }
            _decimalPlaces = value;
            HandleNumberFormatPropertyChanged("DecimalPlaces", _decimalPlaces);
        }
    }

    public bool ShowTrailingDecimalZeros
    {
        get => _showTrailingDecimalZeros;
        set
        {
            if (_showTrailingDecimalZeros == value)
            {
                return;
            }
            _showTrailingDecimalZeros = value;
            HandleNumberFormatPropertyChanged("ShowTrailingDecimalZeros", value);
        }
    }

    public Nullable<NumberStyles> NumberStyle
    {
        get => _numberStyle;
        set
        {
            if (_numberStyle == value)
            {
                return;
            }
            _numberStyle = value;
            HandleNumberFormatPropertyChanged("NumberStyle", value);
        }
    }

    public Nullable<ExponentStyles> ExponentStyle
    {
        get => _exponentStyle;
        set
        {
            if (_exponentStyle == value)
            {
                return;
            }
            _exponentStyle = value;
            HandleNumberFormatPropertyChanged("ExponentStyle", value);
        }
    }

    public Nullable<ComplexStyles> ComplexStyle
    {
        get => _complexStyle;
        set
        {
            if (_complexStyle == value)
            {
                return;
            }
            _complexStyle = value;
            HandleNumberFormatPropertyChanged("ComplexStyle", value);
        }
    }

    public Nullable<int> ZeroThreshold
    {
        get => _zeroThreshold;
        set
        {
            if (_zeroThreshold == value || (value.HasValue && !IsInValidZeroThresholdRange(value)))
            {
                return;
            }
            _zeroThreshold = value;
            HandleNumberFormatPropertyChanged("ZeroThreshold", value);
        }
    }

    public Nullable<int> ExponentialThreshold
    {
        get => _exponentialThreshold;
        set
        {
            if (_exponentialThreshold == value || (value.HasValue && !IsInValidExponentialThresholdRange(value)))
            {
                return;
            }
            _exponentialThreshold = value;
            HandleNumberFormatPropertyChanged("ExponentialThreshold", value);
        }
    }

    public Nullable<int> ComplexThreshold
    {
        get => _complexThreshold;
        set
        {
            if (_complexThreshold == value || (value.HasValue && !IsInValidComplexThresholdRange(value)))
            {
                return;
            }
            _complexThreshold = value;
            HandleNumberFormatPropertyChanged("ComplexThreshold", value);
        }
    }

    public Nullable<RadixStyles> RadixStyle
    {
        get => _radixStyle;
        set
        {
            if (_radixStyle == value)
            {
                return;
            }
            _radixStyle = value;
            HandleNumberFormatPropertyChanged("RadixStyle", value);
        }
    }

    public bool ComplexStyleIsCartesian
    {
        get
        {
            if (ComplexStyle != ComplexStyles.CartesianI)
            {
                return ComplexStyle == ComplexStyles.CartesianJ;
            }
            return true;
        }
    }

    public bool ComplexStyleIsPolar
    {
        get
        {
            if (ComplexStyle != ComplexStyles.PolarDegrees && ComplexStyle != ComplexStyles.PolarRadians)
            {
                return ComplexStyle == ComplexStyles.PolarPiRadians;
            }
            return true;
        }
    }

    [MirrorVerified("PtcControlsCoreNumberFormatMechanicalDifferentialTests")]
    private static bool IsInValidExponentialThresholdRange(Nullable<int> value) => IsNumberInRange(value, 0, 15);

    [MirrorVerified("PtcControlsCoreNumberFormatMechanicalDifferentialTests")]
    private static bool IsInValidZeroThresholdRange(Nullable<int> value) => IsNumberInRange(value, 0, 307);

    [MirrorVerified("PtcControlsCoreNumberFormatMechanicalDifferentialTests")]
    private static bool IsInValidComplexThresholdRange(Nullable<int> value) => IsNumberInRange(value, 0, 307);

    [MirrorVerified("PtcControlsCoreNumberFormatMechanicalDifferentialTests")]
    public event ActionByPropertyNameAndValue FormatChanged;

    [MirrorVerified("PtcControlsCoreNumberFormatMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    private static bool IsValidDecimalPlaces(Nullable<int> value)
    {
        if (!value.HasValue)
        {
            return true;
        }
        int num = value.Value;
        if (num < 0 || num > 15)
        {
            return num == -1;
        }
        return true;
    }

    public static int CoerceDecimalPlaces(int value, int increment)
    {
        if (value == -1)
        {
            if (increment >= 0)
            {
                return -1;
            }
            return Math.Max(0, 16 + increment);
        }
        return Math.Max(0, Math.Min(15, value + increment));
    }

    public bool ShouldBeDisplayedAsScientific(double value)
    {
        if (NumberStyle != NumberStyles.General)
        {
            return false;
        }
        value = Math.Abs(value);
        int num = RadixStyle switch
        {
            RadixStyles.Binary => 2,
            RadixStyles.Octal => 8,
            RadixStyles.Hexadecimal => 16,
            _ => 10,
        };
        bool result;
        if (!ExponentialThreshold.HasValue)
        {
            result = (!DecimalPlaces.HasValue || DecimalPlaces == -1) ? (value < Math.Pow(num, -15.0)) : (value < Math.Pow(num, -DecimalPlaces.Value));
        }
        else
        {
            Nullable<int> num2 = ExponentialThreshold;
            if (num2 == 0)
            {
                num2 = 1;
            }
            Nullable<int> num3 = num2 - 1;
            if (DecimalPlaces == -1)
            {
                num3 = Math.Max(num3.Value, 15);
            }
            result = value < Math.Pow(num, (-num3).Value) || value >= Math.Pow(num, num2.Value);
        }
        return result;
    }

    private static bool IsNumberInRange(Nullable<int> num, int minimum, int maximum)
    {
        if (num.HasValue && num >= minimum)
        {
            return num <= maximum;
        }
        return false;
    }

    private void HandleNumberFormatPropertyChanged(string propertyName, object newValue)
    {
        FirePropertyChanged(propertyName);
        if (FormatChanged != null)
        {
            FormatChanged(propertyName, newValue);
        }
    }

    public bool ShouldBeDisplayedAsZero(double value)
    {
        if (ZeroThreshold.HasValue)
        {
            return Math.Abs(value) < Math.Pow(10.0, -ZeroThreshold.Value);
        }
        return false;
    }

    public void ApplyComplexThreshold(ref double rValue, ref double iValue)
    {
        if (ComplexThreshold.HasValue && rValue != 0.0 && iValue != 0.0)
        {
            double num = Math.Pow(10.0, ComplexThreshold.Value);
            if (Math.Abs(rValue / iValue) > num)
            {
                iValue = 0.0;
            }
            else if (Math.Abs(iValue / rValue) > num)
            {
                rValue = 0.0;
            }
        }
    }

    public NumberFormat()
    {
        DecimalPlaces = 3;
        ShowTrailingDecimalZeros = false;
        NumberStyle = NumberStyles.General;
        ExponentStyle = ExponentStyles.MultiplyByBaseToAPower;
        ComplexStyle = ComplexStyles.CartesianI;
        ZeroThreshold = 15;
        ExponentialThreshold = 3;
        RadixStyle = RadixStyles.Decimal;
        ComplexThreshold = 10;
    }

    public NumberFormat Clone()
    {
        return new NumberFormat
        {
            DecimalPlaces = DecimalPlaces,
            ShowTrailingDecimalZeros = ShowTrailingDecimalZeros,
            NumberStyle = NumberStyle,
            ExponentStyle = ExponentStyle,
            ComplexStyle = ComplexStyle,
            ZeroThreshold = ZeroThreshold,
            ExponentialThreshold = ExponentialThreshold,
            RadixStyle = RadixStyle,
            ComplexThreshold = ComplexThreshold
        };
    }

    public static bool operator ==(NumberFormat format, NumberFormat other)
    {
        if ((object)format == null && (object)other == null)
        {
            return true;
        }
        if ((object)format == null || (object)other == null)
        {
            return false;
        }
        if (format.DecimalPlaces == other.DecimalPlaces &&
            format.ShowTrailingDecimalZeros == other.ShowTrailingDecimalZeros &&
            format.NumberStyle == other.NumberStyle &&
            format.ExponentStyle == other.ExponentStyle &&
            format.ComplexStyle == other.ComplexStyle &&
            format.ZeroThreshold == other.ZeroThreshold &&
            format.ExponentialThreshold == other.ExponentialThreshold &&
            format.ComplexThreshold == other.ComplexThreshold)
        {
            return format.RadixStyle == other.RadixStyle;
        }
        return false;
    }

    public static bool operator !=(NumberFormat format, NumberFormat other)
    {
        return !(format == other);
    }

    public override string ToString()
    {
        return string.Format("{{DecimalPlaces={0},ShowTrailingDecimalZeros={1},NumberStyle={2},ExponentStyle={3},ComplexStyle={4}}}", DecimalPlaces, ShowTrailingDecimalZeros, NumberStyle, ExponentStyle, ComplexStyle);
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public object GetValueByName(string propertyName)
    {
        switch (propertyName)
        {
        case "ComplexStyle":
            return ComplexStyle;
        case "DecimalPlaces":
            return DecimalPlaces;
        case "ExponentialThreshold":
            return ExponentialThreshold;
        case "ExponentStyle":
            return ExponentStyle;
        case "NumberStyle":
            return NumberStyle;
        case "ShowTrailingDecimalZeros":
            return ShowTrailingDecimalZeros;
        case "ZeroThreshold":
            return ZeroThreshold;
        case "RadixStyle":
            return RadixStyle;
        case "ComplexThreshold":
            return ComplexThreshold;
        default:
            DebugTools.FailAssertion(string.Format("{0} property doesn't exist in NumberFormat class.", propertyName));
            return null;
        }
    }

    public void SetValueByName(string propertyName, object value)
    {
        switch (propertyName)
        {
        case "ComplexStyle":
            ComplexStyle = this.ValidateNullableType<ComplexStyles>(value, propertyName);
            return;
        case "ExponentStyle":
            ExponentStyle = this.ValidateNullableType<ExponentStyles>(value, propertyName);
            return;
        case "NumberStyle":
            NumberStyle = this.ValidateNullableType<NumberStyles>(value, propertyName);
            return;
        case "DecimalPlaces":
            DecimalPlaces = this.ValidateNullableType<int>(value, propertyName);
            return;
        case "ZeroThreshold":
            ZeroThreshold = this.ValidateNullableType<int>(value, propertyName);
            return;
        case "ExponentialThreshold":
            ExponentialThreshold = this.ValidateNullableType<int>(value, propertyName);
            return;
        case "ComplexThreshold":
            ComplexThreshold = this.ValidateNullableType<int>(value, propertyName);
            return;
        case "RadixStyle":
            RadixStyle = this.ValidateNullableType<RadixStyles>(value, propertyName);
            return;
        }
        if (value == null)
        {
            return;
        }
        if (propertyName == "ShowTrailingDecimalZeros")
        {
            if (this.IsValidateValueType<bool>(propertyName, value))
            {
                ShowTrailingDecimalZeros = (bool)value;
            }
            return;
        }
    }

    public static NumberFormat GetCommonNumberFormat(IEnumerable<NumberFormat> formats, NumberFormat compareTo)
    {
        return PropertyAggregateExtension.GetCommonFormat(formats, compareTo, "NumberStyle", "ComplexStyle", "DecimalPlaces", "ExponentStyle", "ShowTrailingDecimalZeros", "ZeroThreshold", "ExponentialThreshold", "ComplexThreshold", "RadixStyle");
    }

    protected void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
