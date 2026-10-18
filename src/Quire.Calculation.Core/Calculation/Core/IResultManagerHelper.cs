#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class IResultManagerHelper
{
    [MirrorVerified("PtcCalculationCoreIResultManagerHelperMechanicalDifferentialTests")]
    public static bool IsResultRefIdSpecified(int resultRefId) => resultRefId != -1;
}
