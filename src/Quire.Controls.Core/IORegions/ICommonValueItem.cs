#nullable disable
namespace Quire.Controls.Core.IORegions;

public partial interface ICommonValueItem
{
    IUnitedScalarState ScalarState { get; }
    IUnitedMatrixState MatrixState { get; }
    IStringState StringState { get; }
    IErrorState ErrorState { get; }
}
