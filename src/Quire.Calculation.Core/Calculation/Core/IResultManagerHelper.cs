#nullable disable
using Quire;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class IResultManagerHelper
{
    public const int ResultRefIdMinusOne = -1;

    [MirrorVerified("PtcCalculationCoreIResultManagerHelperMechanicalDifferentialTests")]
    public static bool IsResultRefIdSpecified(int resultRefId) => resultRefId != -1;

    public static bool SetResultIfRefIdSpecified(this IResultManager resultManager, int resultRefId, bool resultRefIdSpecified, ICalculatableResultOwner calc)
    {
        if (!resultRefIdSpecified)
        {
            return false;
        }
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(resultManager != null, null))
        {
            return false;
        }
        resultManager.SetResultByRefId(resultRefId, calc);
        return true;
    }
}
