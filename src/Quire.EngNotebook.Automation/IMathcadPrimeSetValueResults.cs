#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeSetValueResults
{
    int Count { get; }
    int GetResultByIndex(int indexArg);
    int GetResultByAlias(string aliasArg);
}
