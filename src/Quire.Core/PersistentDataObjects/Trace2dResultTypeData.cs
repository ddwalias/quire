#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class Trace2dResultTypeData
{
    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataMechanicalDifferentialTests")]
    public Trace2dResultTypeData() { }

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataMechanicalDifferentialTests")]
    private Trace2dResultTypeDataRangePoints rangePointsField;

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataMechanicalDifferentialTests")]
    public Trace2dResultTypeDataRangePoints RangePoints { get => rangePointsField; set => rangePointsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataMechanicalDifferentialTests")]
    private Trace2dResultTypeDataResultPoints resultPointsField;

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeDataMechanicalDifferentialTests")]
    public Trace2dResultTypeDataResultPoints ResultPoints { get => resultPointsField; set => resultPointsField = value; }
}
