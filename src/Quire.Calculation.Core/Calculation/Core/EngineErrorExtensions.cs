#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class EngineErrorExtensions
{
    private const string _pholderErrorCode = "bad%_placeholder";

    public static bool IsPlaceholderError(this EngineError error)
    {
        if (error != null)
        {
            return error.ErrorCode == "bad%_placeholder";
        }
        return false;
    }
}
