#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    public bool SaveDocumentAsPdf_ForApiOnly(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string pdfFileName)
    {
        return document.SaveAsPdf_ForApiOnly(pdfFileName);
    }

    private void HandleSaveDocumentAs(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        SaveDocumentWithFilename(ActiveDocument, e.Parameter as string);
    }

    internal bool SaveDocumentWithFilename(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string fileName)
    {
        return SaveDocumentWithFilenameAndExtension(
            document,
            fileName,
            null,
            global::Quire.Controls.RegularSaveOptions.Empty);
    }

    internal bool SaveDocumentAsNonPrimeFormat_ForApiOnly(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string fileName)
    {
        return SaveDocumentWithFilenameAndExtension(
            document,
            fileName,
            null,
            global::Quire.Controls.RegularSaveOptions.NonPrimeFormatForApi);
    }

    private void HandleSaveDocumentCopy(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        SaveDocumentWithFilenameAndExtension(
            ActiveDocument,
            e.Parameter as string,
            null,
            global::Quire.Controls.DetachedCopyOptions.Empty);
    }

    internal bool SaveAndPushEngNote(
        global::Quire.Controls.Core.IEngineeringDocument engineeringDocument)
    {
        return SaveDocument(
            engineeringDocument,
            global::Quire.Controls.RegularSaveOptions.WithStatesGeneration);
    }

    private void HandleSaveDocumentWithInputsOutputsStates(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs eventArgs)
    {
        SaveDocument(ActiveDocument, global::Quire.Controls.RegularSaveOptions.WithStatesGeneration);
    }

    private void HandleSaveDocumentWithoutInputsOutputsStates(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs eventArgs)
    {
        SaveDocumentWithoutInputsOutputsStates(ActiveDocument);
    }

    internal bool SaveDocumentWithoutInputsOutputsStates(
        global::Quire.Controls.Core.IEngineeringDocument document)
    {
        return SaveDocument(document, global::Quire.Controls.RegularSaveOptions.Empty);
    }

    internal global::System.Collections.Generic.IList<global::Quire.Controls.Core.FileDialogFilterPair>
        GetFileSaveOpenFilters(string extension, bool save)
    {
        return GetFileSaveOpenFilters(
            extension,
            TryFindResource,
            global::Quire.Services.FunctionalityService.Instance,
            global::Quire.Properties.Settings.Default.DocOptionsEnabled,
            save);
    }

}
