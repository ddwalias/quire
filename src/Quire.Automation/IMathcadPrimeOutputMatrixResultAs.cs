#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeOutputMatrixResultAs
{
    int ErrorCode { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
}
