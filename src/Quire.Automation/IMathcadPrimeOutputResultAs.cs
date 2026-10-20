#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeOutputResultAs
{
    int ErrorCode { get; }
    double RealResult { get; }
}
