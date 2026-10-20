#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeInputMatrixResult
{
    int ErrorCode { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
    string Units { get; }
}
