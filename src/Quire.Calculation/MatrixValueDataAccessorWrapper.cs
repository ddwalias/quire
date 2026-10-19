#nullable disable
using System.Collections;
using Quire.Automation;
using Quire.Mirror;

namespace Quire.Calculation;

internal partial class MatrixValueDataAccessorWrapper
{
    [MirrorVerified("PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests")]
    private IMatrixValueDataAccessor AccessorProxy { get; set; }

    [MirrorVerified("PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests")]
    public MatrixValueDataAccessorWrapper(IMatrixValueDataAccessor accessor)
    {
        AccessorProxy = accessor;
    }

    [MirrorVerified("PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests")]
    public IList<IList> GetColumnsPlainDataValues(int startIndex, int endIndex) =>
        AccessorProxy.GetColumnsPlainDataValues(startIndex, endIndex);

    [MirrorVerified("PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests")]
    public int GetFirstNonPlainDataColumnIndex() => AccessorProxy.GetFirstNonPlainDataColumnIndex();

    [MirrorVerified("PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests")]
    public int GetFirstNonPlainDataRowIndex() => AccessorProxy.GetFirstNonPlainDataRowIndex();

    [MirrorVerified("PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests")]
    public IList<IList> GetRowsPlainDataValues(int startIndex, int endIndex) =>
        AccessorProxy.GetRowsPlainDataValues(startIndex, endIndex);
}
