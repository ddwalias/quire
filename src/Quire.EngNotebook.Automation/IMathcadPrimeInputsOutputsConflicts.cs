#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeInputsOutputsConflicts
{
    void AddGeneralWarning(string warningArg);
    void AddGeneralError(string errorArg);
    void AddItemWarning(string aliasArg, string warningArg);
    void AddItemError(string aliasArg, string errorArg);
}
