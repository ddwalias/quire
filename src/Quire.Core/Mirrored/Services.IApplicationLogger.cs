#nullable disable
#pragma warning disable

namespace Quire.Services;

partial interface IApplicationLogger
{
    string LogFolder { get; set; }
    string WriteToLog(string logType, string logContents) ;
    string GetListOfLoadedModulesData() ;
    string GetEnvironmentVariablesAndCommandLineArgumentsData() ;
    string GetLastApplicationLogPath() ;
    string GetApplicationStartTime() ;
    string GetMiscInfo() ;
    string GetSystemInfoIfAvailable() ;
}
