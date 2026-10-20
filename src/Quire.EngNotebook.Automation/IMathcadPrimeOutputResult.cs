#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeOutputResult
{
    int ErrorCode { get; }
    double RealResult { get; }
    string Units { get; }
}
