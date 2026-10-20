#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeGetValueResult
{
    ValueResultTypes ResultType { get; }
    int ErrorCode { get; }
    string Units { get; }
    double RealResult { get; }
    string StringResult { get; }
    IMathcadPrimeMatrix MatrixResult { get; }
}
