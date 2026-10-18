#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class DevAssertionException
{
    [MirrorVerified("PtcDevAssertionExceptionMechanicalDifferentialTests")]
    public DevAssertionException(string message) : base(message) { }

    [MirrorVerified("PtcDevAssertionExceptionMechanicalDifferentialTests")]
    public DevAssertionException(string message, Exception innerException) : base(message, innerException) { }
}
