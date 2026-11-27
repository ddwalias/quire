#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private void HandleNewDocument(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions =
            e.Parameter as global::Quire.Controls.Core.DocumentReadonlyOptions;
        NewDocument(readonlyOptions);
    }

    private void HandleNewDocumentFromExisting(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        string templateFileName = GetTemplateFileName(args, isNewFromTemplate: true);
        if (string.IsNullOrEmpty(templateFileName))
        {
            return;
        }

        global::Quire.Controls.Core.IEngineeringDocument document;
        if (!global::System.IO.File.Exists(templateFileName))
        {
            OpenFileExceptionHandler(
                templateFileName,
                new global::System.IO.FileNotFoundException(string.Empty, templateFileName));
        }
        else if (HandleClonningAndOpenningWorksheetInNewDocument(templateFileName, null, out document) &&
                 document.ReadonlyOptions.FileLocationHistoryEnabled)
        {
            MruFileManager.AddMRUFileItemIfNeeded(templateFileName);
        }
    }

    private bool HandleClonningAndOpenningWorksheetInNewDocument(
        string fileName,
        global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions,
        out global::Quire.Controls.Core.IEngineeringDocument document)
    {
        bool result = true;
        document = NewDocument(incrementCounter: true, readonlyOptions, isTemplate: false);
        global::System.Exception exception = CloneWorksheetAndOpenInDocument(document, fileName);
        if (exception != null)
        {
            CloseDocument(document);
            result = false;
            OpenFileExceptionHandler(fileName, exception);
        }

        return result;
    }

    private void HandleOpenTemplateDocument(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        if (!UseDocumentTabs && !AskToSaveDocument(ActiveDocument))
        {
            return;
        }

        string templateFileName = GetTemplateFileName(args);
        if (string.IsNullOrEmpty(templateFileName))
        {
            return;
        }

        if (!string.IsNullOrEmpty(templateFileName))
        {
            if (!global::System.IO.File.Exists(templateFileName))
            {
                OpenFileExceptionHandler(
                    templateFileName,
                    new global::System.IO.FileNotFoundException(string.Empty, templateFileName));
            }
            else
            {
                OpenFile(templateFileName, null);
            }
        }
    }

    private global::System.Exception OpenFileInDocument(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string fileName,
        string tag)
    {
        global::Quire.Controls.Core.FileLoadResult fileLoadResult;
        try
        {
            global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;
            document.IsCalculationsEnabled = false;
            document.IsAutoCalcOn = false;
            fileLoadResult = document.OpenFileAndUpdateCWDState(fileName);
            if (fileLoadResult.LoadException == null)
            {
                if (document.ReadonlyOptions.FileLocationHistoryEnabled)
                {
                    MruFileManager.AddMRUFileItemIfNeeded(fileName);
                }

                Addons.CallAddonsWhenDocumentOpened(document, fileName);
            }

            document.IsAutoCalcOn = document.DocCalculationOptions.IsAutoCalcOn;
            document.IsCalculationsEnabled = true;
            document.IsDirty = false;
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
            ScrollToTagInDoc(document, tag);
        }

        return fileLoadResult.LoadException;
    }

}
