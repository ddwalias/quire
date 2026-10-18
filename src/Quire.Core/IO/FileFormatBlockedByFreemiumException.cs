#nullable disable
using Quire.Mirror;

namespace Quire.IO;

public partial class FileFormatBlockedByFreemiumException
{
    [MirrorVerified("PtcIOFileFormatBlockedByFreemiumExceptionMechanicalDifferentialTests")]
    public FileFormatBlockedByFreemiumException() : base("The file cannot be saved in Express version, since this format is a Premium feature of PTC Mathcad.") { }
}
