#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class RequestShowErrorEventArgs
{
    [MirrorVerified("PtcCalculationCoreRequestShowErrorEventArgsConstructorTests")]
    public RequestShowErrorEventArgs(EngineError error)
    {
        Error = error;
    }

    [MirrorVerified("PtcCalculationCoreRequestShowErrorEventArgsMechanicalDifferentialTests")]
    public EngineError Error { get; private set; }
}
