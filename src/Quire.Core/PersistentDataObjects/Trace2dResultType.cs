#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class Trace2dResultType
{
    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    public Trace2dResultType() { }

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    private AxisInfoType rangeInfoField;

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    public AxisInfoType RangeInfo { get => rangeInfoField; set => rangeInfoField = value; }

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    private AxisInfoType resultInfoField;

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    public AxisInfoType ResultInfo { get => resultInfoField; set => resultInfoField = value; }

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    private Trace2dResultTypeData dataField;

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    public Trace2dResultTypeData Data { get => dataField; set => dataField = value; }

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    private TraceResultTypeEnum traceTypeField;

    [MirrorVerified("PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests")]
    public TraceResultTypeEnum TraceType { get => traceTypeField; set => traceTypeField = value; }
}
