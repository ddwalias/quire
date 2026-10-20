#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeOutputMatrixResult
{
    int ErrorCode { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
    string Units { get; }
}
