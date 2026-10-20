#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeOutputResultAs
{
    int ErrorCode { get; }
    double RealResult { get; }
}
