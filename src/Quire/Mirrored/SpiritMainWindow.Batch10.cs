#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private void OpenFile(
        string fileName,
        global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions,
        string tag = "")
    {
        string extension = global::System.IO.Path.GetExtension(fileName);
        if (UseDocumentTabs)
        {
            foreach (global::Quire.Controls.Core.IEngineeringDocument openDocument in OpenDocumentsList)
            {
                if (openDocument.FilePath != null &&
                    global::System.IO.Path.GetFullPath(openDocument.FilePath).Equals(
                        global::System.IO.Path.GetFullPath(fileName),
                        global::System.StringComparison.InvariantCultureIgnoreCase))
                {
                    ActiveDocument = openDocument;
                    ScrollToTagInDoc(openDocument, tag);
                    return;
                }
            }
        }

        global::Quire.Controls.Core.IEngineeringDocument document =
            NewDocument(
                incrementCounter: false,
                readonlyOptions,
                string.Equals(extension, ".mctx", global::System.StringComparison.OrdinalIgnoreCase));
        TryOpenFileInDocument(document, fileName, tag);
    }

    private bool TryOpenFileInDocument(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string fileName,
        string tag = "")
    {
        global::System.Exception exception = OpenFileInDocument(document, fileName, tag);
        if (exception != null)
        {
            CloseDocument(document);
            OpenFileExceptionHandler(fileName, exception);
            return false;
        }

        return true;
    }

    private void HandleOpenFileFailure(string fileName, global::System.Exception result)
    {
        bool logException = false;
        string displayedMessage;
        if (result is global::System.IO.FileNotFoundException)
        {
            displayedMessage = (string)TryFindResource("Application.FileDoesNotExist") + "\n" + fileName;
        }
        else if (result is global::System.FormatException)
        {
            logException = true;
            displayedMessage = (string)TryFindResource("Application.FileTypeNotSupported");
        }
        else if (result is global::Quire.Core.Serialization.IncompatibleFormatWorksheetException)
        {
            displayedMessage =
                (TryFindResource("Application.FileNotCompatibleKey") as string) ?? result.Message;
        }
        else if (result is global::Quire.Controls.PreviousVersionWorksheetException)
        {
            displayedMessage = (string)TryFindResource("Application.OpenPreviousVersionWorksheet");
        }
        else
        {
            logException = true;
            displayedMessage = (string)TryFindResource("Application.FileTypeNotSupported");
        }

        if (logException)
        {
            result.Data.Add("File being opened", fileName);
            string logFile = ExLogger.LogException(result);
            DisplayCriticalWarningWithLogFile(displayedMessage, logFile);
        }
        else
        {
            DisplayWarning(displayedMessage);
        }
    }

    private void HandleSaveDocumentWithExtension(string extension)
    {
        global::Quire.Controls.Core.IEngineeringDocument activeDocument = ActiveDocument;
        if (activeDocument.IsPrinting && !AskCancelPrintInProgressDialog())
        {
            return;
        }

        SaveDocumentWithFilenameAndExtension(
            activeDocument,
            null,
            extension,
            global::Quire.Controls.RegularSaveOptions.Empty);
    }

    internal static global::System.Collections.Generic.IList<global::Quire.Controls.Core.FileDialogFilterPair>
        GetFileSaveOpenFilters(
            string extension,
            global::System.Func<object, object> tryFindResource,
            global::Quire.Services.IFunctionalityService functionalityService,
            bool docOptionsEnabled,
            bool save)
    {
        global::System.Collections.Generic.List<global::Quire.Controls.Core.FileDialogFilterPair> filters =
            new global::System.Collections.Generic.List<global::Quire.Controls.Core.FileDialogFilterPair>();
        if (extension == ".mcdx" || extension == null)
        {
            filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair
            {
                FilterString = string.Format(
                    "{0} (*{1})|*{1}",
                    tryFindResource(save
                        ? "Application.MathcadPrimeWorksheetSaveFilter"
                        : "Application.MathcadPrimeWorksheetsOpenFilter"),
                    ".mcdx"),
                Extension = ".mcdx"
            });
        }

        if (save)
        {
            if (extension == ".mctx" ||
                (extension == null && functionalityService.IsFunctionalityEnabled(
                    global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.SaveEngineeringDocumentAsMCTXCommand)))
            {
                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair
                {
                    FilterString = string.Format(
                        "{0} (*{1})|*{1}",
                        tryFindResource("Application.MathcadPrimeTemplateSaveFilter"),
                        ".mctx"),
                    Extension = ".mctx"
                });
            }

            if (extension == ".rtf" ||
                (extension == null && functionalityService.IsFunctionalityEnabled(
                    global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.SaveEngineeringDocumentAsRtfCommand)))
            {
                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair
                {
                    FilterString = string.Format(
                        "{0} (*{1})|*{1}",
                        tryFindResource("Application.RtfDocumentSaveFilter"),
                        ".rtf"),
                    Extension = ".rtf"
                });
            }

            if (extension == ".pdf" ||
                (extension == null && functionalityService.IsFunctionalityEnabled(
                    global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.SaveEngineeringDocumentAsPdfCommand)))
            {
                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair
                {
                    FilterString = string.Format(
                        "{0} (*{1})|*{1}",
                        tryFindResource("Application.PdfDocumentSaveFilter"),
                        ".pdf"),
                    Extension = ".pdf"
                });
            }

            if (extension == ".xps" || extension == null)
            {
                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair
                {
                    FilterString = string.Format(
                        "{0} (*{1})|*{1}",
                        tryFindResource("Application.XPSDocumentSaveFilter"),
                        ".xps"),
                    Extension = ".xps"
                });
            }

            if (extension == ".dita" || ((extension == null) & docOptionsEnabled))
            {
                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair
                {
                    FilterString = string.Format("{0} (*{1})|*{1}", "Dita files", ".dita"),
                    Extension = ".dita"
                });
            }
        }
        else if (extension == ".mctx" ||
                 (extension == null && functionalityService.IsFunctionalityEnabled(
                     global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.OpenTemplateDocumentCommand)))
        {
            filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair
            {
                FilterString = string.Format(
                    "{0} (*{1})|*{1}",
                    tryFindResource("Application.MathcadPrimeTemplatesOpenFilter"),
                    ".mctx"),
                Extension = ".mctx"
            });
            if (extension == null)
            {
                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair
                {
                    FilterString = string.Format(
                        "{0} (*{1};*{2})|*{1};*{2}",
                        tryFindResource("Application.AllMathcadPrimeFilesOpenFilter"),
                        ".mcdx",
                        ".mctx"),
                    Extension = ".*"
                });
            }
        }

        return filters;
    }
}
