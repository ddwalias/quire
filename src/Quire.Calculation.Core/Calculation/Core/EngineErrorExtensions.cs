#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class EngineErrorExtensions
{
    [MirrorVerified("PtcCalculationCoreEngineErrorExtensionsMechanicalDifferentialTests")]
    public static bool IsPlaceholderError(this EngineError error)
    {
        if (error != null)
        {
            return error.ErrorCode == _pholderErrorCode;
        }

        return false;
    }
}
