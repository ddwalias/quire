#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeSetValueResults
{
    int Count { get; }
    int GetResultByIndex(int indexArg);
    int GetResultByAlias(string aliasArg);
}
