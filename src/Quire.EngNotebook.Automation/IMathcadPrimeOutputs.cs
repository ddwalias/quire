#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeOutputs
{
    int Count { get; }
    string GetAliasByIndex(int indexArg);
}
