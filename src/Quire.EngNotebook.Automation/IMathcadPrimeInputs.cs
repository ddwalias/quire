#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeInputs
{
    int Count { get; }
    string GetAliasByIndex(int indexArg);
}
