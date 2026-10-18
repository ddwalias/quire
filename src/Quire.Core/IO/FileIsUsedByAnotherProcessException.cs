#nullable disable
using System.IO;
using Quire.Mirror;

namespace Quire.IO;

public partial class FileIsUsedByAnotherProcessException
{
    [MirrorVerified("PtcIOFileIsUsedByAnotherProcessExceptionMechanicalDifferentialTests")]
    public FileIsUsedByAnotherProcessException(IOException innerException) : base("File Is Used By Another Process", innerException) { }
}
