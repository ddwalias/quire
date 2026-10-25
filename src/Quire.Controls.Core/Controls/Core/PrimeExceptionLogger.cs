#nullable disable
using System;
using System.Collections.Generic;
using System.Text;
using Quire.License;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class PrimeExceptionLogger : ExceptionLogger, IPrimeExceptionLogger, IExceptionLogger, IApplicationLogger, IService
{
    public Func<IEnumerable<string>> OpenFilesListGetter { private get; set; }

    public override string[] GetExceptionData(IEnumerable<Exception> exceptions)
    {
        string exceptionInfo = GetExceptionInfo(exceptions);
        try
        {
            return new string[9]
            {
                GetMiscInfo(),
                exceptionInfo,
                GetApplicationStartTime(),
                GetSystemInfoIfAvailable(),
                GetApplicationMemoryUsage(),
                GetOpenFilesData(),
                GetUndoStackData(),
                GetEnvironmentVariablesAndCommandLineArgumentsData(),
                GetListOfLoadedModulesData()
            };
        }
        catch (Exception ex)
        {
            return new string[2]
            {
                exceptionInfo,
                string.Format("... (Exception when getting additional data: {0}) ", ex.Message)
            };
        }
    }

    public string GetOpenFilesData()
    {
        StringBuilder openFilesData = new StringBuilder();
        openFilesData.AppendLine("The following files are being opened:");
        try
        {
            OpenFilesListGetter().Each(delegate(string fileName)
            {
                openFilesData.AppendLine(fileName);
            });
        }
        catch (Exception ex)
        {
            openFilesData.AppendLine(string.Format("... (Exception when getting files list: {0})", ex.Message));
        }
        return openFilesData.ToString();
    }

    public string GetUndoStackData()
    {
        StringBuilder undoStackData = new StringBuilder();
        IUndoableCommand[] undoCommands = CommandBuffer.CurrentInstance.GetUndoCommands();
        if (undoCommands.Length != 0)
        {
            undoStackData.AppendLine("The following commands are in Undo Stack:");
            undoCommands.Each(delegate(IUndoableCommand command)
            {
                undoStackData.AppendLine(command.Name);
            });
        }
        return undoStackData.ToString();
    }

    private string GetLicenseTypeData()
    {
        string result = "License data is not available";
        if (LicenseManager.Instance != null)
        {
            result = string.Format("Current license type is: {0}", LicenseManager.Instance.LicenseType);
        }
        return result;
    }

    private static string GetApplicationVersionData()
    {
        return string.Format("Application version is: {0}", WpfUtils.ApplicationName);
    }

    public override string GetMiscInfo()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetMiscInfo());
        stringBuilder.AppendLine(GetApplicationVersionData());
        stringBuilder.AppendLine(GetLicenseTypeData());
        return stringBuilder.ToString();
    }
}
