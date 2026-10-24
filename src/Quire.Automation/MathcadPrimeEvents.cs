#nullable disable

namespace Quire.Automation;

[global::Quire.Mirror.MirrorOf("Ptc.MathcadPrime.Automation.MathcadPrimeEvents, Ptc.MathcadPrime.Automation")]
public enum MathcadPrimeEvents
{
    OnExit = 100,
    OnWorksheetSaved = 0,
    OnWorksheetClosed = 1,
    OnWorksheetModified = 2,
    OnWorksheetRenamed = 3,
    OnWorksheetInputsOutputsSelected = 4,
    OnWorksheetStatesGenerated = 5,
    OnWorksheetStatesGenerating = 6,
    OnRequestToUpdateInputs = 7,
}
