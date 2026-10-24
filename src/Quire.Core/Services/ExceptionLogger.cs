#nullable disable
using System;
using Quire.Mirror;

namespace Quire.Services;

public partial class ExceptionLogger
{
    [MirrorVerified("PtcServicesExceptionLoggerMechanicalDifferentialTests")]
    public ExceptionLogger() { }

    public string LogException(Exception exception)
    {
        return LogExceptions(new Exception[1] { exception });
    }
}
