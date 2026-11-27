// Mathcad IL-order whole-class partial of Spirit.CMathcadPrimeApplication (MathcadPrime 12.0.1.0).
// Unfinished members are unmarked throw placeholders in Mathcad order.
#nullable disable
#pragma warning disable

using Quire.Automation;

namespace Quire;

public partial class CMathcadPrimeApplication
{
    private global::Quire.Automation.IMathcadPrimeWorksheets _worksheets;
    private bool _isSubscribedApplicationEvents;
    private global::Quire.Automation.IMathcadPrimeEvents _events;
    private global::Quire.Automation.IMathcadPrimeEvents2 _events2;
    private readonly global::System.Collections.Generic.ICollection<global::Quire.Automation.MathcadPrimeEvents> _subscribedEvents2;
    private readonly bool _isUiInteractiveModeDisabled;

    private bool IsEventsInitialized => _events != null;

    private bool IsEventsInitialized2 => _events2 != null;

    private void SubscribesApplicationEvents() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void UnsubscribesApplicationEvents() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnMathcadPrimeExit(object sender, global::System.Windows.ExitEventArgs e)
    {
        FireOnExitEvent();
        FireOnExitEvent2();
    }

    private void OnDocumentSaved(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument)
    {
        if (engineeringDocument != null)
        {
            FireOnWorksheetSavedEvent(engineeringDocument.FilePath);
            FireOnWorksheetSavedEvent2(engineeringDocument.FilePath);
        }
    }

    private void OnDocumentIOSelected(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, global::System.Collections.Generic.List<string> inputsList, global::System.Collections.Generic.List<string> outputsList)
    {
        FireOnWorksheetInputsOutputsSelectedEvent(inputsList, outputsList);
        FireOnWorksheetInputsOutputsSelectedEvent2(engineeringDocument, inputsList, outputsList);
    }

    private void OnDocumentClosed(string documentFullName, string documentName)
    {
        FireOnWorksheetClosedEvent2(documentFullName, documentName);
    }

    private void OnDocumentModified(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument)
    {
        FireOnWorksheetModifiedEvent2(engineeringDocument);
    }

    private void OnDocumentRenamed(string previousFullName, string currentFullName, string previousDocName, string currentDocName)
    {
        FireOnWorksheetRenamedEvent2(previousFullName, currentFullName, previousDocName, currentDocName);
    }

    private void OnDocumentInputsOutputsStatesGeneratedWhenSave(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, global::Quire.Controls.Core.IORegions.IInputOutputItemsStates inputsOutputsStates, global::Quire.Controls.IO.IInputOutputItemsConflicts conflicts)
    {
        FireOnWorksheetStatesGeneratedEvent2(engineeringDocument, WorksheetOperations.Save, inputsOutputsStates, conflicts);
    }

    private void OnDocumentInputsOutputsStatesGeneratingBeforeSave(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, global::Quire.Controls.Core.IORegions.IInputOutputItemsStates inputsOutputsStates, global::Quire.Controls.IO.IInputOutputItemsConflicts conflicts)
    {
        FireOnWorksheetStatesGeneratingEvent2(engineeringDocument, WorksheetOperations.Save, inputsOutputsStates, conflicts);
    }

    private void OnRequestToUpdateInputs(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, global::Quire.Controls.Core.IORegions.IValuesSetter setter)
    {
        FireOnRequestToUpdateInputsEvent2(engineeringDocument, setter);
    }

    public CMathcadPrimeApplication()
    {
        _subscribedEvents2 = new List<MathcadPrimeEvents>();
        _isUiInteractiveModeDisabled = true;
    }

    internal CMathcadPrimeApplication(bool isUiInteractiveModeDisabled)
    {
        if (isUiInteractiveModeDisabled)
        {
            _subscribedEvents2 = new List<MathcadPrimeEvents>();
        }

        _isUiInteractiveModeDisabled = isUiInteractiveModeDisabled;
    }

    public void ReleaseComEvents() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool Visible { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private bool DoGetVisible() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetVisible(bool visibleMode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Activate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoActivate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Quit(global::Quire.Automation.SaveOption saveOptionArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoTryToCloseMathcadPrime() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeWorksheet ActiveWorksheet { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::Quire.Automation.IMathcadPrimeWorksheet DoGetActiveWorksheet() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeWorksheet Open(string documentPathArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private string ShowPopupError(string documentPath, global::Quire.Automation.IMathcadPrimeWorksheetReadonlyOptions readonlyOptions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Automation.IMathcadPrimeWorksheet DoOpen(string documentPath, global::Quire.Automation.IMathcadPrimeWorksheetReadonlyOptions readonlyOptions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IEngineeringDocument GetOpenedDocument(string documentFullName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int InitializeEvents(global::Quire.Automation.IMathcadPrimeEvents eventsArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetInputsOutputsSelectedEvent(global::System.Collections.Generic.List<string> inputsList, global::System.Collections.Generic.List<string> outputsList) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetSavedEvent(string documentName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnExitEvent() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeWorksheets Worksheets { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public void CloseAll(global::Quire.Automation.SaveOption saveOptionArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoCloseAll(global::Quire.Automation.SaveOption saveOption) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string GetVersion() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int InitializeEvents2(global::Quire.Automation.IMathcadPrimeEvents2 eventsArg, bool subscribeAllArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int SubscribeEvent(global::Quire.Automation.MathcadPrimeEvents primeEventArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int UnsubscribeEvent(global::Quire.Automation.MathcadPrimeEvents primeEventArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetSavedEvent2(string documentName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetClosedEvent2(string documentPath, string documentName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetModifiedEvent2(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetRenamedEvent2(string previousFullName, string currentFullName, string previousDocName, string currentDocName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetStatesGeneratedEvent2(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, global::Quire.Automation.WorksheetOperations operation, global::Quire.Controls.Core.IORegions.IInputOutputItemsStates inputsOutputsStates, global::Quire.Controls.IO.IInputOutputItemsConflicts conflicts) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetStatesGeneratingEvent2(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, global::Quire.Automation.WorksheetOperations operation, global::Quire.Controls.Core.IORegions.IInputOutputItemsStates inputsOutputsStates, global::Quire.Controls.IO.IInputOutputItemsConflicts conflicts) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnRequestToUpdateInputsEvent2(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, global::Quire.Controls.Core.IORegions.IValuesSetter setter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnWorksheetInputsOutputsSelectedEvent2(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, global::System.Collections.Generic.List<string> inputsList, global::System.Collections.Generic.List<string> outputsList) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireOnExitEvent2() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeWorksheetReadonlyOptions CreateWorksheetReadonlyOptions() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeWorksheet3 OpenEx(string documentPathArg, global::Quire.Automation.IMathcadPrimeWorksheetReadonlyOptions optionsArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

}
