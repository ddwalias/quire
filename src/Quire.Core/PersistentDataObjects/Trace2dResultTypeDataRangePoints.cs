#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class Trace2dResultTypeDataRangePoints
{
    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataRangePointsMechanicalDifferentialTests")]
    public Trace2dResultTypeDataRangePoints() { }

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataRangePointsMechanicalDifferentialTests")]
    private DataVectorsType dataVectorsField;

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataRangePointsMechanicalDifferentialTests")]
    public DataVectorsType DataVectors { get => dataVectorsField; set => dataVectorsField = value; }
}
