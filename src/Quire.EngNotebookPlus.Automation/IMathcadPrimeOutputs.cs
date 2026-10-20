#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeOutputs
{
    int Count { get; }
    string GetAliasByIndex(int indexArg);
}
