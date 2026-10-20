#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public sealed partial class EngineError
{
    [MirrorVerified("PtcCalculationCoreEngineErrorRelationsDifferentialTests")]
    public bool IsEquivalentTo(EngineError error, bool checkNodeHandle = true)
    {
        if (error == null)
        {
            return false;
        }
        if (ErrorCode == error.ErrorCode
            && ((checkNodeHandle && NodeHandle == error.NodeHandle) || !checkNodeHandle)
            && Argument == error.Argument
            && ((ErrorSource == null && error.ErrorSource == null)
                || (ErrorSource != null && ErrorSource.Equals(error.ErrorSource)))
            && ((OriginalError == null && error.OriginalError == null)
                || (OriginalError != null && OriginalError.IsEquivalentTo(error.OriginalError))))
        {
            return true;
        }
        return false;
    }

    [MirrorVerified("PtcCalculationCoreEngineErrorRelationsDifferentialTests")]
    public EngineError FindNextError(EngineError error)
    {
        if (error == null)
        {
            return null;
        }
        EngineError engineError = this;
        while (engineError != null && !error.IsEquivalentTo(engineError.OriginalError))
        {
            engineError = engineError.OriginalError;
        }
        return engineError;
    }

    [MirrorVerified("PtcCalculationCoreEngineErrorRelationsDifferentialTests")]
    public bool IsAncestorOf(EngineError error)
    {
        if (error == null)
        {
            return false;
        }
        for (EngineError engineError = this; engineError != null; engineError = engineError.OriginalError)
        {
            if (error.IsEquivalentTo(engineError))
            {
                return true;
            }
        }
        return false;
    }
}
