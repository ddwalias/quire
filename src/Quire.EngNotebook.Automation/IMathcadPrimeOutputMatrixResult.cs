#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeOutputMatrixResult
{
    int ErrorCode { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
    string Units { get; }
}
