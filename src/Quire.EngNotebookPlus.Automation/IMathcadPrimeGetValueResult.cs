#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeGetValueResult
{
    ValueResultTypes ResultType { get; }
    int ErrorCode { get; }
    string Units { get; }
    double RealResult { get; }
    string StringResult { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
}
