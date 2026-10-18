#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class FileManager
{
    [MirrorVerified("PtcControlsCoreFileManagerMechanicalDifferentialTests")]
    public static bool IsPackage(string path, string extension) => IsPackage(path, extension, false);

    [MirrorVerified("PtcControlsCoreFileManagerMechanicalDifferentialTests")]
    public FileManager() { }

    partial class Extension
    {
        [MirrorVerified("PtcControlsCoreFileManagerMechanicalDifferentialTests")]
        public Extension() { }
    }
}
