#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeInputResult
{
    int ErrorCode { get; }
    double RealResult { get; }
    string Units { get; }
}
