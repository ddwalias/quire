#nullable disable
using Quire.Mirror;

namespace Quire.Services;

public partial class ApplicationBehaviorConstant
{
    public const int DefaultTimeToWaitForResultInSec = 60;
    public const int WaitIndefinitely = -1;

    [MirrorVerified("PtcServicesApplicationBehaviorConstantMechanicalDifferentialTests")]
    public ApplicationBehaviorConstant() { }
}
