#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeOutputMatrixResultAs
{
    int ErrorCode { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
}
