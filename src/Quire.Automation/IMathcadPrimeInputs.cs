#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeInputs
{
    int Count { get; }
    string GetAliasByIndex(int indexArg);
}
