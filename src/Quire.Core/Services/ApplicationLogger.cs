#nullable disable
using System.Diagnostics;
using System.Text.RegularExpressions;
using Quire.Mirror;
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

    [MirrorVerified("Release IL")]
    public string GetApplicationMemoryUsage() => string.Format("Memory in use : {0:0.##} MB total, {1:0.##} MB managed", (double)Process.GetCurrentProcess().PrivateMemorySize64 / Math.Pow(2.0, 20.0), (double)GC.GetTotalMemory(false) / Math.Pow(2.0, 20.0));

    [MirrorVerified("Release IL")]
    private static string ReplaceMultiSpaceWithSingleSpace(string stringToReplace)
    {
        Regex regex = new Regex("\\s+");
        return regex.Replace(stringToReplace, " ");
    }
}
