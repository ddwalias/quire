#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class PrimeExceptionLogger
{
    [MirrorVerified("PtcControlsCorePrimeExceptionLoggerMechanicalDifferentialTests")]
    public PrimeExceptionLogger() { }

    [MirrorVerified("PtcControlsCorePrimeExceptionLoggerMechanicalDifferentialTests")]
    public Func<IEnumerable<string>> OpenFilesListGetter { private get; set; }
}
