#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeEvents2
{
    void OnWorksheetSaved(string documentFullNameArg);
    void OnWorksheetClosed(string documentFullNameArg, string documentNameArg);
    void OnWorksheetModified(string documentFullNameArg, string documentNameArg, bool isModifiedArg);
    void OnWorksheetRenamed(string previousFullNameArg, string currentFullNameArg, string previousDocNameArg, string currentDocNameArg);
    void OnWorksheetInputsOutputsSelected(string documentFullNameArg, string documentNameArg, IMathcadPrimeInputs inputsArg, IMathcadPrimeOutputs outputsArg);
    void OnExit();
    void OnWorksheetStatesGenerated(string documentFullNameArg, string documentNameArg, WorksheetOperations operationsArg, IMathcadPrimeInputsOutputsStates itemsStatesArg, IMathcadPrimeInputsOutputsConflicts conflictsArg);
    void OnWorksheetStatesGenerating(string documentFullNameArg, string documentNameArg, WorksheetOperations operationsArg, IMathcadPrimeInputsOutputsStates itemsStatesArg, IMathcadPrimeInputsOutputsConflicts conflictsArg);
    void OnWorksheetRequestToUpdateInputs(string documentFullNameArg, string documentNameArg, IMathcadPrimeValuesSetter setterArg);
    void Ping();
}
