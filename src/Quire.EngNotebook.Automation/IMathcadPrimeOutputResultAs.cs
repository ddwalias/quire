#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeOutputResultAs
{
    int ErrorCode { get; }
    double RealResult { get; }
}
