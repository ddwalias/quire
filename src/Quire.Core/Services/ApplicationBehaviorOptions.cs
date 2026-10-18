#nullable disable
using Quire.Mirror;

namespace Quire.Services;

public partial class ApplicationBehaviorOptions
{
    [MirrorVerified("PtcServicesApplicationBehaviorOptionsMechanicalDifferentialTests")]
    private int _collectInputOutputStatesTimeout;

    [MirrorVerified("PtcServicesApplicationBehaviorOptionsMechanicalDifferentialTests")]
    public int CollectInputOutputStatesTimeout { get => _collectInputOutputStatesTimeout; set => _collectInputOutputStatesTimeout = value; }

    [MirrorVerified("PtcServicesApplicationBehaviorOptionsMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }
}
