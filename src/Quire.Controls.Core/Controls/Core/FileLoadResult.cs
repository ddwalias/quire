#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class FileLoadResult
{
    [MirrorVerified("PtcControlsCoreFileLoadResultMechanicalDifferentialTests")]
    public FileLoadResult() { }

    [MirrorVerified("PtcControlsCoreFileLoadResultMechanicalDifferentialTests")]
    public Exception LoadException { get; set; }

    [MirrorVerified("PtcControlsCoreFileLoadResultMechanicalDifferentialTests")]
    public bool ErrorsOccurredWhenLoadingRegions { get; set; }

    [MirrorVerified("PtcControlsCoreFileLoadResultMechanicalDifferentialTests")]
    public IEnumerable<Exception> DeserializationExceptions { get; set; }
}
