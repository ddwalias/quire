#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class MatrixOfDoubles
{
    [MirrorVerified("PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests")]
    public MatrixOfDoubles() { }

    [MirrorVerified("PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests")]
    private double[] cellsField;

    [MirrorVerified("PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests")]
    public double[] Cells { get => cellsField; set => cellsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests")]
    private string rowsField;

    [MirrorVerified("PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests")]
    public string Rows { get => rowsField; set => rowsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests")]
    private string columnsField;

    [MirrorVerified("PtcPersistentDataObjectsMatrixOfDoublesMechanicalDifferentialTests")]
    public string Columns { get => columnsField; set => columnsField = value; }
}
