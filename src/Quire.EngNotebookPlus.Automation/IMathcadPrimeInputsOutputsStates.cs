#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeInputsOutputsStates
{
    int ScalarInputsCount { get; }
    int ScalarOutputsCount { get; }
    int GetInputScalarStateByIndex(int indexArg, out string aliasArg, out double valueArg, out string unitsArg);
    int GetOutputScalarStateByIndex(int indexArg, out string aliasArg, out double valueArg, out string unitsArg);
    int GetInputScalarStateByAlias(string aliasArg, out double valueArg, out string unitsArg);
    int GetOutputScalarStateByAlias(string aliasArg, out double valueArg, out string unitsArg);
}
