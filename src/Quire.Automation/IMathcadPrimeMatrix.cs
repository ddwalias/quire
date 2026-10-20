#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeMatrix
{
    double[][] Matrix { get; }
    double Rows { get; }
    double Columns { get; }
    int SetMatrixElement(int rowIndexArg, int columnIndexArg, double valueArg);
    int GetMatrixElement(int rowIndexArg, int columnIndexArg, out double valueArg);
}
