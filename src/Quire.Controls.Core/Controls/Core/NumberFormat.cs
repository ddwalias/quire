#nullable disable
using System.ComponentModel;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NumberFormat
{
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
