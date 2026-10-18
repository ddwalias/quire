#nullable disable
using Quire.Mirror;

namespace Quire.Services;

public partial class ApplicationLogger
{
    [MirrorVerified("PtcServicesApplicationLoggerMechanicalDifferentialTests")]
    protected string _applicationLogFilePath;

    [MirrorVerified("PtcServicesApplicationLoggerMechanicalDifferentialTests")]
    public string GetLastApplicationLogPath() => _applicationLogFilePath;

    [MirrorVerified("PtcServicesApplicationLoggerMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }
}
