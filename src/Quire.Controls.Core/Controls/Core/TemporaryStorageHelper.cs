#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class TemporaryStorageHelper
{
    [MirrorVerified("PtcControlsCoreTemporaryStorageHelperMechanicalDifferentialTests")]
    public TemporaryStorageHelper() { }

    [MirrorVerified("PtcControlsCoreTemporaryStorageHelperMechanicalDifferentialTests")]
    public string StorageFileName { get; set; }
}
