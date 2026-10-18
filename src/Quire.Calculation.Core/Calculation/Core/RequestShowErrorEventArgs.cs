#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class RequestShowErrorEventArgs
{
    [MirrorVerified("PtcCalculationCoreRequestShowErrorEventArgsMechanicalDifferentialTests")]
    public EngineError Error { get; private set; }
}
