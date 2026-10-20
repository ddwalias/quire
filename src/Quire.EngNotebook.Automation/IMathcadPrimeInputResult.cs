#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeInputResult
{
    int ErrorCode { get; }
    double RealResult { get; }
    string Units { get; }
}
