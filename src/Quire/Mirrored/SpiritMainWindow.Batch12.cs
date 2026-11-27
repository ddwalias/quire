#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    public string LaunchOpenFileDialogAndGetFileNameForTemplates(
        string templatesFolderLocation,
        bool overrideDialogTitle,
        string title = null)
    {
        string result = string.Empty;
        string dialogTitle = title ?? (overrideDialogTitle
            ? TryFindResource("Application.FileOpenDialogForTemplatesTitle") as string
            : TryFindResource("Application.FileOpenDialogForOpenTemplatesTitle") as string);
        string filterString = GetFileSaveOpenFilters(".mctx", save: false)[0].FilterString;
        if (!global::System.IO.Directory.Exists(templatesFolderLocation))
        {
            if (!IsDirectoryOneOfDefaultTemplateLocations(templatesFolderLocation))
            {
                SetAndDisplayWarning(
                    TryFindResource("Application.DirectoryDoesNotExist") as string,
                    templatesFolderLocation);
                return null;
            }

            if (global::Quire.Services.AppServiceRegistry
                    .GetService<global::Quire.IO.IFileOperationsProvider>()
                    .CreateDirectory(templatesFolderLocation) != null)
            {
                SetAndDisplayWarning(
                    TryFindResource("Application.UnableCreateDirectory") as string,
                    templatesFolderLocation);
                return null;
            }
        }

        if (global::Quire.SystemInfo.OsBeforeXp())
        {
            var openFileDialog = new global::Microsoft.Win32.OpenFileDialog
            {
                Filter = filterString,
                Title = dialogTitle,
                InitialDirectory = templatesFolderLocation
            };
            if (DialogManager.ShowDialog(openFileDialog) == true)
            {
                result = openFileDialog.FileName;
            }
        }
        else
        {
            var openFileDialog = new global::System.Windows.Forms.OpenFileDialog
            {
                Filter = filterString,
                Title = dialogTitle,
                InitialDirectory = templatesFolderLocation,
                RestoreDirectory = true
            };
            if (DialogManager.ShowDialog(openFileDialog) ==
                global::System.Windows.Forms.DialogResult.OK)
            {
                result = openFileDialog.FileName;
            }
        }

        return result;
    }

    private global::System.Exception CloneWorksheetAndOpenInDocument(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string fileName)
    {
        global::Quire.Controls.Core.FileLoadResult fileLoadResult;
        try
        {
            global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;
            document.IsCalculationsEnabled = false;
            document.IsAutoCalcOn = false;
            fileLoadResult = document.NewFromDocument(fileName);
            document.IsCalculationsEnabled = true;
            document.IsAutoCalcOn = document.DocCalculationOptions.IsAutoCalcOn;
            if (fileLoadResult.ErrorsOccurredWhenLoadingRegions)
            {
                string logFile = ExLogger.LogExceptions(fileLoadResult.DeserializationExceptions);
                DisplayCriticalWarningWithLogFile(
                    (string)TryFindResource("Application.SomeRegionsCorrupted"),
                    logFile);
            }
        }
        finally
        {
            global::System.Windows.Input.Mouse.OverrideCursor = null;
        }

        return fileLoadResult.LoadException;
    }

    private void HandleSaveFileFailure(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string fileName,
        global::System.Exception result,
        global::Quire.Controls.Core.IDocumentSaveOptions options)
    {
        if (MessageBoxManager.IsDisabled)
        {
            return;
        }

        if (HandleMissingWriteRightsOnSaveException(document, result, fileName, options))
        {
            return;
        }

        if (result is global::Quire.IO.PrintToFileException printToFileException)
        {
            string message;
            if (printToFileException.ErrorCode == global::Quire.IO.PrintFileErrorCode.TakeInnerExceptionByOS)
            {
                message = printToFileException.InnerException?.Message;
            }
            else
            {
                message = TryFindResource(printToFileException.ErrorCode) as string;
                if (message.IsNotEmpty())
                {
                    string formattedMessage = null;
                    try
                    {
                        formattedMessage = string.Format(message, printToFileException.MessageArgs);
                    }
                    catch
                    {
                        global::Quire.DebugTools.FailAssertion(null, global::System.Array.Empty<object>());
                    }

                    message = formattedMessage ?? message;
                }
            }

            DisplayWarning(
                global::Quire.DebugTools.DevAssert(message.IsNotEmpty(), null).Passed
                    ? message
                    : (result.Message ?? string.Empty));
        }
        else if (result is global::Quire.IO.FileFormatBlockedByFreemiumException)
        {
            if (!MessageBoxManager.IsDisabled)
            {
                global::Quire.DebugTools.FailAssertionDoNotThrow(null, global::System.Array.Empty<object>());
            }
        }
        else if (result is global::System.ApplicationException)
        {
            DisplayWarning(result.Message);
        }
        else
        {
            result.Data.Add("File being saved", fileName);
            string logFile = ExLogger.LogException(result);
            DisplayCriticalWarningWithLogFile(
                (string)TryFindResource("Application.FileTypeNotSupported"),
                logFile);
        }
    }
}
