#nullable disable
namespace Quire.Controls.Core.IORegions;

public partial interface IUnitedMatrixState
{
    double[][] Value { get; }
    string Units { get; }
}
