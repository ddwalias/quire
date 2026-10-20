#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeOutputMatrixResultAs
{
    int ErrorCode { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
}
