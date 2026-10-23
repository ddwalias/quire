#nullable disable
#pragma warning disable

namespace Quire.Services;

partial class ApplicationLogger
{
    private static string GetExceptionLogLocation(string appConfigFolderPath)
    {
        string text = global::System.IO.Path.Combine(appConfigFolderPath, "Logs");
        if (!_fileOperationsProvider.DirectoryExists(text))
        {
            _fileOperationsProvider.CreateDirectory(text);
        }
        return text;
    }

    private string _fullProductName = _product;
    private string _logFolder = _defaultApplicationLogFolderPath;

    public ApplicationLogger() { }

    public string GetApplicationStartTime()
    {
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
        string value = string.Format("{0} started at:", FullProductName);
        global::System.Diagnostics.Process processById = global::System.Diagnostics.Process.GetProcessById(global::System.Diagnostics.Process.GetCurrentProcess().Id);
        stringBuilder.AppendLine(value);
        stringBuilder.AppendLine(processById.StartTime.ToString("hh:mm:ss MM/dd/yyyy"));
        return stringBuilder.ToString();
    }

    public virtual string GetMiscInfo()
    {
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
        string arg = global::Quire.Xml.XmlWorksheetData.BuildId ?? "Unknown";
        stringBuilder.AppendLine(string.Format("Build Number : {0}", arg));
        string value = string.Format("System Culture : {0}; Current UI Culture : {1}", global::System.Globalization.CultureInfo.InstalledUICulture, global::System.Threading.Thread.CurrentThread.CurrentUICulture);
        stringBuilder.AppendLine(value);
        stringBuilder.AppendLine(string.Format("Architecture: {0}", global::Quire.Xml.XmlWorksheetData.Architecture));
        return stringBuilder.ToString();
    }

    protected global::System.IO.TextWriter CreateApplicationLog(string logType)
    {
        _applicationLogFilePath = global::System.IO.Path.Combine(LogFolder, string.Format("{0}_{1}_{2}.log", FullProductName, logType, global::System.DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        global::System.IO.TextWriter fileWriter = _fileOperationsProvider.GetFileWriter(_applicationLogFilePath);
        string msg = string.Format("File location : {0}", _applicationLogFilePath);
        WriteToLog(msg, fileWriter);
        return fileWriter;
    }

    public string SubProduct { get => _subProduct; set { _subProduct = value; FullProductName = string.IsNullOrEmpty(value) ? _product : string.Format("{0}_{1}", _product, value); } }
    protected string FullProductName { get => _fullProductName; private set => _fullProductName = string.IsNullOrEmpty(value) ? _product : value; }
    public string LogFolder { get => _logFolder; set => _logFolder = string.IsNullOrEmpty(value) ? _defaultApplicationLogFolderPath : value; }
}
