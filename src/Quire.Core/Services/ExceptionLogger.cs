#nullable disable
using System;
using System.Collections.Generic;
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

    public virtual string[] GetExceptionData(IEnumerable<Exception> exceptions)
    {
        return new string[5]
        {
            GetMiscInfo(),
            GetExceptionInfo(exceptions),
            GetApplicationStartTime(),
            GetEnvironmentVariablesAndCommandLineArgumentsData(),
            GetListOfLoadedModulesData()
        };
    }
}
