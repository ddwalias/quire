#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeOutputs
{
    int Count { get; }
    string GetAliasByIndex(int indexArg);
}
