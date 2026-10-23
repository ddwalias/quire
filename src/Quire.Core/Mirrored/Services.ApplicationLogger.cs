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

    public string SubProduct { get => _subProduct; set { _subProduct = value; FullProductName = string.IsNullOrEmpty(value) ? _product : string.Format("{0}_{1}", _product, value); } }
    protected string FullProductName { get => _fullProductName; private set => _fullProductName = string.IsNullOrEmpty(value) ? _product : value; }
    public string LogFolder { get => _logFolder; set => _logFolder = string.IsNullOrEmpty(value) ? _defaultApplicationLogFolderPath : value; }
}
