#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeInputs
{
    int Count { get; }
    string GetAliasByIndex(int indexArg);
}
