#nullable disable
using System;
using System.Collections;
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

    public string GetSystemInfoIfAvailable()
    {
        StringBuilder stringBuilder = new StringBuilder();
        string value;
        string items;
        Process process = Process.Start(new ProcessStartInfo("systeminfo.exe")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        });
        using (StreamReader streamReader = process.StandardOutput)
        {
            value = streamReader.ReadToEnd();
        }
        using (StreamReader streamReader2 = process.StandardError)
        {
            items = streamReader2.ReadToEnd();
        }
        if (global::Quire.EnumerableExtensions.IsNullOrEmpty<char>(items))
        {
            stringBuilder.AppendLine("Output of systeminfo.exe utility: ");
            stringBuilder.Append(value);
        }
        return stringBuilder.ToString();
    }

    public string WriteToLog(string logType, string logContents)
    {
        try
        {
            TextWriter textWriter = CreateApplicationLog(logType);
            try
            {
                new string[4]
                {
                    GetMiscInfo(),
                    logContents,
                    GetApplicationStartTime(),
                    GetEnvironmentVariablesAndCommandLineArgumentsData()
                }.Each((string data) =>
                {
                    WriteToLog(data, textWriter);
                });
            }
            finally
            {
                if (textWriter != null)
                {
                    ((IDisposable)textWriter).Dispose();
                }
            }
        }
        catch (Exception)
        {
        }
        return _applicationLogFilePath;
    }

    public string GetEnvironmentVariablesAndCommandLineArgumentsData()
    {
        StringBuilder environmentData = new StringBuilder();
        string value = string.Format("{0} was executed with following command line arguments:", FullProductName);
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        string value2 = string.Format("Dump of system environment variables: ", default(ReadOnlySpan<object>));
        IDictionary environmentVariables = Environment.GetEnvironmentVariables();
        if (commandLineArgs.Length > 1)
        {
            environmentData.AppendLine(value);
            commandLineArgs.Cast<string>().Each((string commandLineArgument) =>
            {
                environmentData.AppendLine(commandLineArgument);
            });
        }
        if (environmentVariables.Count > 0)
        {
            environmentData.AppendLine(value2);
            environmentVariables.Cast<DictionaryEntry>().Each((DictionaryEntry entry) =>
            {
                environmentData.AppendLine(string.Format("{0}={1}", entry.Key, entry.Value));
            });
        }
        return environmentData.ToString();
    }

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
