#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private bool SaveDocumentWithFilenameAndExtension(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string fileName,
        string extension,
        global::Quire.Controls.Core.IDocumentSaveOptions options)
    {
        if (!options.IsHandlingException)
        {
            Addons.CallAddonsWhenDocumentSaving(document.FilePath);
        }
        else
        {
            global::Quire.DebugTools.DevAssert(fileName.IsNotEmpty(), null);
        }

        if (string.IsNullOrEmpty(fileName))
        {
            fileName = AskForSaveDefaultFileName(document, extension);
        }

        extension = global::System.IO.Path.GetExtension(fileName) ?? extension ?? ".mcdx";
        document.IsDocumentATemplate = extension.Equals(
            ".mctx",
            global::System.StringComparison.OrdinalIgnoreCase);
        return TrySaveFile(document, fileName, options);
    }

    private bool SaveDocument(
        global::Quire.Controls.Core.IEngineeringDocument document,
        global::Quire.Controls.RegularSaveOptions options)
    {
        bool result = false;
        string fileName = document.FilePath;
        if (global::Quire.DebugTools.DevAssert(!options.IsHandlingException, null).Passed)
        {
            Addons.CallAddonsWhenDocumentSaving(fileName);
        }

        int promptForFileName = ShouldPromptForFileNameOnSave(document);
        if (promptForFileName != -1)
        {
            if (promptForFileName == 1 && !document.IsNetworkDriveDisconnected)
            {
                fileName = AskForSaveDefaultFileName(document, null);
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                result = TrySaveFile(document, fileName, options);
            }
        }

        return result;
    }

    private string AskForSaveDefaultFileName(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string defaultFileExtensionForDialog)
    {
        if (defaultFileExtensionForDialog == null)
        {
            defaultFileExtensionForDialog = ".mcdx";
            if (!string.IsNullOrEmpty(document.FileName))
            {
                string extension = global::System.IO.Path.GetExtension(document.FileName);
                if (!string.IsNullOrEmpty(extension) && extension.Equals(
                        ".mctx",
                        global::System.StringComparison.OrdinalIgnoreCase))
                {
                    defaultFileExtensionForDialog = ".mctx";
                }
            }
        }

        return AskForSaveFileName(
            document,
            GetFileSaveOpenFilters(null, save: true),
            defaultFileExtensionForDialog);
    }

}
