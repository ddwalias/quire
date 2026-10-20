#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeInputMatrixResult
{
    int ErrorCode { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
    string Units { get; }
}
