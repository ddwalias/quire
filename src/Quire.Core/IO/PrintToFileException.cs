#nullable disable
using Quire.Mirror;

namespace Quire.IO;

public partial class PrintToFileException
{
    [MirrorVerified("PtcIOPrintToFileExceptionTierADifferentialTests")]
    public PrintToFileException(string message, global::System.Exception innerException, PrintFileErrorCode errorCode,
        params object[] messageMessageArgs)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        MessageArgs = messageMessageArgs ?? new object[0];
    }
}
