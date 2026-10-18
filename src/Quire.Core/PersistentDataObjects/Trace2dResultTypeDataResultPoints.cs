#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class Trace2dResultTypeDataResultPoints
{
    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataResultPointsMechanicalDifferentialTests")]
    public Trace2dResultTypeDataResultPoints() { }

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataResultPointsMechanicalDifferentialTests")]
    private DataVectorsType dataVectorsField;

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataResultPointsMechanicalDifferentialTests")]
    public DataVectorsType DataVectors { get => dataVectorsField; set => dataVectorsField = value; }
}
