#nullable disable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Quire;
using Quire.Mirror;

namespace Quire.Services;
public partial class ApplicationLogger
{
    [MirrorVerified("PtcServicesApplicationLoggerMechanicalDifferentialTests")]
    protected string _applicationLogFilePath;

    public string GetListOfLoadedModulesData()
    {
        StringBuilder loadedModulesData = new StringBuilder();
        string value = string.Format("The following Modules were loaded by {0}:", FullProductName);
        string loadedModuleListFormat = string.Join(Environment.NewLine, new string[3] { "Module Name: {0}", "{1}", "Module Memory Size: {2}" });
        ProcessModuleCollection modules = Process.GetProcessById(Process.GetCurrentProcess().Id).Modules;
        if (modules.Count > 0)
        {
            loadedModulesData.Append(value);
            modules.Cast<ProcessModule>().Each((ProcessModule module) =>
            {
                loadedModulesData.AppendLine(string.Format(loadedModuleListFormat, module.ModuleName, ReplaceMultiSpaceWithSingleSpace(module.FileVersionInfo.ToString().Replace(Environment.NewLine, "; ")), module.ModuleMemorySize));
            });
        }
        return loadedModulesData.ToString();
    }

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

    protected void WriteToLog(string msg, TextWriter textWriter)
    {
        if (string.IsNullOrWhiteSpace(msg))
        {
            return;
        }
        textWriter.WriteLine(msg);
        textWriter.WriteLine();
        textWriter.WriteLine();
    }

}
