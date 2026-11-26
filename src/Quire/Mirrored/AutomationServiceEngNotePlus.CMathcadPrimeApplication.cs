using System;
using System.Collections.Generic;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.IO;
using Quire.EngNotebookPlus.Automation;
using Quire.Xml;

namespace Quire.AutomationServiceEngNotePlus;

public partial class CMathcadPrimeApplication
{
    private IMathcadPrimeWorksheets _worksheets;
    private bool _isSubscribedApplicationEvents;
    private IMathcadPrimeEvents _events;
    private IMathcadPrimeEvents2 _events2;
    private readonly ICollection<MathcadPrimeEvents> _subscribedEvents2;
    private readonly bool _isUiInteractiveModeDisabled;
    private System.Timers.Timer _timer;
    private bool _isSubscribedPing;
    private static int _isClosing;
    private static bool _isPinging;

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

    private bool IsEventsInitialized => _events != null;

    private bool IsEventsInitialized2 => _events2 != null;

    public IMathcadPrimeWorksheet ActiveWorksheet
    {
        get
        {
            try
            {
                DelegateFunction0<IMathcadPrimeWorksheet> getActiveWorksheet = DoGetActiveWorksheet;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getActiveWorksheet, DispatcherPriority.Normal) as IMathcadPrimeWorksheet;
            }
            catch
            {
            }

            return null;
        }
    }

    public IMathcadPrimeWorksheets Worksheets
    {
        get
        {
            try
            {
                return _worksheets ??= new CMathcadPrimeWorksheets(_isUiInteractiveModeDisabled);
            }
            catch
            {
            }

            return null;
        }
    }

    private bool DoGetVisible()
    {
        try
        {
            return MathcadPrimeComServerManager.Instance.PrimeWindow.IsVisible;
        }
        catch
        {
        }

        return false;
    }

    private void DoSetVisible(bool visibleMode)
    {
        try
        {
            if (visibleMode)
            {
                MathcadPrimeComServerManager.Instance.PrimeWindow.Show();
            }
            else
            {
                MathcadPrimeComServerManager.Instance.PrimeWindow.Hide();
            }
        }
        catch
        {
        }
    }

    public void Activate()
    {
        try
        {
            Delegate0 activate = DoActivate;
            MathcadPrimeComServerManager.Instance.InvokeDelegate(activate, DispatcherPriority.Normal);
        }
        catch
        {
        }
    }

    private void DoActivate()
    {
        // The original re-reads PrimeWindow for every call; no local is kept.
        try
        {
            if (MathcadPrimeComServerManager.Instance.PrimeWindow.WindowState == WindowState.Minimized)
            {
                MathcadPrimeComServerManager.Instance.PrimeWindow.WindowState = WindowState.Normal;
            }

            MathcadPrimeComServerManager.Instance.PrimeWindow.Activate();
            if (!MathcadPrimeComServerManager.Instance.PrimeWindow.Topmost)
            {
                MathcadPrimeComServerManager.Instance.PrimeWindow.Topmost = true;
                MathcadPrimeComServerManager.Instance.PrimeWindow.Topmost = false;
            }
        }
        catch
        {
        }
    }

    private IMathcadPrimeWorksheet DoGetActiveWorksheet()
    {
        try
        {
            IEngineeringDocument activeDocument = MathcadPrimeComServerManager.Instance.PrimeWindow.ActiveDocument;
            if (activeDocument != null)
            {
                return new CMathcadPrimeWorksheet(activeDocument, _isUiInteractiveModeDisabled);
            }
        }
        catch
        {
        }

        return null;
    }

    public string GetVersion()
    {
        try
        {
            return XmlWorksheetData.CurrentAppVersion.ToString();
        }
        catch
        {
        }

        return null;
    }

    public IMathcadPrimeWorksheetReadonlyOptions CreateWorksheetReadonlyOptions()
    {
        try
        {
            return new CMathcadPrimeWorksheetReadonlyOptions();
        }
        catch
        {
        }

        return null;
    }

    private void OnMathcadPrimeExit(object sender, ExitEventArgs e)
    {
        FireOnExitEvent();
        FireOnExitEvent2();
    }

    private void OnDocumentSaved(IEngineeringDocument engineeringDocument)
    {
        if (engineeringDocument != null)
        {
            FireOnWorksheetSavedEvent(engineeringDocument.FilePath);
            FireOnWorksheetSavedEvent2(engineeringDocument.FilePath);
        }
    }

    private void OnDocumentIOSelected(IEngineeringDocument engineeringDocument, List<string> inputsList, List<string> outputsList)
    {
        FireOnWorksheetInputsOutputsSelectedEvent(inputsList, outputsList);
        FireOnWorksheetInputsOutputsSelectedEvent2(engineeringDocument, inputsList, outputsList);
    }

    private void OnDocumentClosed(string documentFullName, string documentName)
    {
        FireOnWorksheetClosedEvent2(documentFullName, documentName);
    }

    private void OnDocumentModified(IEngineeringDocument engineeringDocument)
    {
        FireOnWorksheetModifiedEvent2(engineeringDocument);
    }

    private void OnDocumentRenamed(string previousFullName, string currentFullName, string previousDocName, string currentDocName)
    {
        FireOnWorksheetRenamedEvent2(previousFullName, currentFullName, previousDocName, currentDocName);
    }

    private void OnDocumentInputsOutputsStatesGeneratedWhenSave(IEngineeringDocument engineeringDocument, IInputOutputItemsStates inputsOutputsStates, IInputOutputItemsConflicts conflicts)
    {
        FireOnWorksheetStatesGeneratedEvent2(engineeringDocument, WorksheetOperations.Save, inputsOutputsStates, conflicts);
    }

    private void OnDocumentInputsOutputsStatesGeneratingBeforeSave(IEngineeringDocument engineeringDocument, IInputOutputItemsStates inputsOutputsStates, IInputOutputItemsConflicts conflicts)
    {
        FireOnWorksheetStatesGeneratingEvent2(engineeringDocument, WorksheetOperations.Save, inputsOutputsStates, conflicts);
    }

    private void OnRequestToUpdateInputs(IEngineeringDocument engineeringDocument, IValuesSetter setter)
    {
        FireOnRequestToUpdateInputsEvent2(engineeringDocument, setter);
    }

    private void FireOnWorksheetInputsOutputsSelectedEvent(List<string> inputsList, List<string> outputsList)
    {
        try
        {
            if (IsEventsInitialized)
            {
                IMathcadPrimeInputs inputs = new Inputs(new AutomationSpiritInputs(inputsList));
                IMathcadPrimeOutputs outputs = new Outputs(new AutomationSpiritOutputs(outputsList));
                _events.OnSelect(inputs, outputs);
            }
        }
        catch
        {
        }
    }

    private void FireOnWorksheetSavedEvent(string documentName)
    {
        try
        {
            if (IsEventsInitialized)
            {
                _events.OnSave(documentName);
            }
        }
        catch
        {
        }
    }

    private void FireOnExitEvent()
    {
        try
        {
            if (IsEventsInitialized)
            {
                _events.OnExit();
            }
        }
        catch
        {
        }
    }

    private void FireOnWorksheetSavedEvent2(string documentName)
    {
        try
        {
            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnWorksheetSaved))
            {
                _events2.OnWorksheetSaved(documentName);
            }
        }
        catch
        {
        }
    }

    private void FireOnWorksheetClosedEvent2(string documentPath, string documentName)
    {
        try
        {
            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnWorksheetClosed))
            {
                _events2.OnWorksheetClosed(documentPath, documentName);
            }
        }
        catch
        {
        }
    }

    private void FireOnWorksheetModifiedEvent2(IEngineeringDocument engineeringDocument)
    {
        try
        {
            if (engineeringDocument == null)
            {
                return;
            }

            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnWorksheetModified))
            {
                _events2.OnWorksheetModified(engineeringDocument.FilePath, engineeringDocument.DocumentName, engineeringDocument.IsDirty);
            }
        }
        catch
        {
        }
    }

    private void FireOnWorksheetRenamedEvent2(string previousFullName, string currentFullName, string previousDocName, string currentDocName)
    {
        try
        {
            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnWorksheetRenamed))
            {
                _events2.OnWorksheetRenamed(previousFullName, currentFullName, previousDocName, currentDocName);
            }
        }
        catch
        {
        }
    }

    private void FireOnRequestToUpdateInputsEvent2(IEngineeringDocument engineeringDocument, IValuesSetter setter)
    {
        try
        {
            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnRequestToUpdateInputs))
            {
                _events2.OnWorksheetRequestToUpdateInputs(engineeringDocument.FilePath, engineeringDocument.DocumentName, new ValuesSetter(new AutomationSpiritValuesSetter(setter)));
            }
        }
        catch
        {
        }
    }

    private void FireOnExitEvent2()
    {
        try
        {
            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnExit))
            {
                _events2.OnExit();
            }
        }
        catch
        {
        }
    }

    private IEngineeringDocument GetOpenedDocument(string documentFullName)
    {
        IEngineeringDocument openedDocument = null;
        IList<IEngineeringDocument> openDocuments = MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList;
        for (int i = openDocuments.Count - 1; i > -1; i--)
        {
            IEngineeringDocument document = openDocuments[i];
            if ((string.IsNullOrEmpty(documentFullName) && document.FilePath == null)
                || (document.FilePath != null && document.FilePath.Equals(documentFullName, StringComparison.OrdinalIgnoreCase)))
            {
                openedDocument = document;
                break;
            }
        }

        return openedDocument;
    }

    private void DoCloseAll(SaveOption saveOption)
    {
        try
        {
            if (MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList != null)
            {
                for (int i = MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList.Count; i > 0; i--)
                {
                    IEngineeringDocument activeDocument = MathcadPrimeComServerManager.Instance.PrimeWindow.ActiveDocument;
                    if (activeDocument != null)
                    {
                        // The original closes the worksheet through IMathcadPrimeWorksheet2.
                        ((IMathcadPrimeWorksheet2)new CMathcadPrimeWorksheet(activeDocument, _isUiInteractiveModeDisabled)).Close(saveOption);
                        if (MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList.Contains(activeDocument))
                        {
                            break;
                        }
                    }
                }
            }
        }
        catch
        {
        }
    }

    public int InitializeEvents(IMathcadPrimeEvents eventsArg)
    {
        try
        {
            SubscribesApplicationEvents();
            _events = eventsArg;
            if (IsEventsInitialized)
            {
                return 0;
            }
        }
        catch
        {
        }

        return 1;
    }

    public int InitializeEvents2(IMathcadPrimeEvents2 eventsArg, bool subscribeAllArg)
    {
        try
        {
            SubscribesApplicationEvents();
            _events2 = eventsArg;
            if (IsEventsInitialized2)
            {
                if (subscribeAllArg)
                {
                    foreach (MathcadPrimeEvents primeEvent in Enum.GetValues(typeof(MathcadPrimeEvents)))
                    {
                        if (primeEvent != MathcadPrimeEvents.Ping && !_subscribedEvents2.Contains(primeEvent))
                        {
                            _subscribedEvents2.Add(primeEvent);
                        }
                    }
                }

                return 0;
            }
        }
        catch
        {
        }

        return 1;
    }

    public int SubscribeEvent(MathcadPrimeEvents primeEventArg)
    {
        try
        {
            if (IsEventsInitialized2 && Enum.IsDefined(typeof(MathcadPrimeEvents), primeEventArg))
            {
                if (!_subscribedEvents2.Contains(primeEventArg))
                {
                    _subscribedEvents2.Add(primeEventArg);
                }

                if (primeEventArg == MathcadPrimeEvents.Ping)
                {
                    try
                    {
                        _timer = new System.Timers.Timer(5000);
                        _timer.Elapsed += OnPingClient;
                        _isSubscribedPing = true;
                        _timer.Start();
                    }
                    catch
                    {
                    }
                }

                return 0;
            }
        }
        catch
        {
        }

        return 1;
    }

    public int UnsubscribeEvent(MathcadPrimeEvents primeEventArg)
    {
        try
        {
            if (IsEventsInitialized2 && Enum.IsDefined(typeof(MathcadPrimeEvents), primeEventArg))
            {
                if (_subscribedEvents2.Contains(primeEventArg))
                {
                    _subscribedEvents2.Remove(primeEventArg);
                }

                if (primeEventArg == MathcadPrimeEvents.Ping)
                {
                    try
                    {
                        if (_timer != null)
                        {
                            _timer.Stop();
                            _timer.Elapsed -= OnPingClient;
                            _timer.Dispose();
                        }
                    }
                    catch
                    {
                    }

                    _isSubscribedPing = false;
                }

                return 0;
            }
        }
        catch
        {
        }

        return 1;
    }

    private void FireOnWorksheetStatesGeneratedEvent2(IEngineeringDocument engineeringDocument, WorksheetOperations operation, IInputOutputItemsStates inputsOutputsStates, IInputOutputItemsConflicts conflicts)
    {
        try
        {
            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnWorksheetStatesGenerated))
            {
                _events2.OnWorksheetStatesGenerated(engineeringDocument.FilePath, engineeringDocument.DocumentName, operation, new InputsOutputsStates(new AutomationSpiritInputsOutputsStates(inputsOutputsStates)), new InputsOutputsConflicts(new AutomationSpiritInputsOutputsConflicts(conflicts)));
            }
        }
        catch
        {
        }
    }

    private void FireOnWorksheetStatesGeneratingEvent2(IEngineeringDocument engineeringDocument, WorksheetOperations operation, IInputOutputItemsStates inputsOutputsStates, IInputOutputItemsConflicts conflicts)
    {
        try
        {
            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnWorksheetStatesGenerating))
            {
                _events2.OnWorksheetStatesGenerating(engineeringDocument.FilePath, engineeringDocument.DocumentName, operation, new InputsOutputsStates(new AutomationSpiritInputsOutputsStates(inputsOutputsStates)), new InputsOutputsConflicts(new AutomationSpiritInputsOutputsConflicts(conflicts)));
            }
        }
        catch
        {
        }
    }

    private void FireOnWorksheetInputsOutputsSelectedEvent2(IEngineeringDocument engineeringDocument, List<string> inputsList, List<string> outputsList)
    {
        try
        {
            if (engineeringDocument == null)
            {
                return;
            }

            if (IsEventsInitialized2 && _subscribedEvents2.Contains(MathcadPrimeEvents.OnWorksheetInputsOutputsSelected))
            {
                IMathcadPrimeInputs inputs = new Inputs(new AutomationSpiritInputs(inputsList));
                IMathcadPrimeOutputs outputs = new Outputs(new AutomationSpiritOutputs(outputsList));
                _events2.OnWorksheetInputsOutputsSelected(engineeringDocument.FilePath, engineeringDocument.DocumentName, inputs, outputs);
            }
        }
        catch
        {
        }
    }

    private string ShowPopupError(string documentPath, IMathcadPrimeWorksheetReadonlyOptions readonlyOptions)
    {
        try
        {
            AppCommands.ShowSaveAsError.Execute(true, MathcadPrimeComServerManager.Instance.PrimeWindow);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
        }

        return null;
    }

    public void ReleaseComEvents()
    {
        try
        {
            if (IsEventsInitialized)
            {
                if (Marshal.IsComObject(_events))
                {
                    Marshal.FinalReleaseComObject(_events);
                }

                _events = null;
            }
        }
        catch
        {
        }

        try
        {
            if (IsEventsInitialized2)
            {
                try
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Elapsed -= OnPingClient;
                        _timer.Dispose();
                    }
                }
                catch
                {
                }

                _isSubscribedPing = false;
                try
                {
                    if (Marshal.IsComObject(_events2))
                    {
                        Marshal.FinalReleaseComObject(_events2);
                    }
                }
                catch
                {
                }

                _events2 = null;
            }
        }
        catch
        {
        }

        UnsubscribesApplicationEvents();
        _subscribedEvents2?.Clear();
    }
}
