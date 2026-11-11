#nullable disable
using System.ComponentModel;
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

    private Nullable<int> _decimalPlaces;
    private bool _showTrailingDecimalZeros;
    private Nullable<NumberStyles> _numberStyle;
    private Nullable<ExponentStyles> _exponentStyle;
    private Nullable<ComplexStyles> _complexStyle;
    private Nullable<int> _zeroThreshold;
    private Nullable<int> _exponentialThreshold;
    private Nullable<int> _complexThreshold;
    private Nullable<RadixStyles> _radixStyle;

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
}
