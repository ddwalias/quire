#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeOutputResult
{
    int ErrorCode { get; }
    double RealResult { get; }
    string Units { get; }
}
