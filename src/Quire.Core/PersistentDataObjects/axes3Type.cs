#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axes3Type
{
    [MirrorVerified("PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests")]
    public axes3Type() { }

    [MirrorVerified("PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests")]
    private xAxis xAxisField;

    [MirrorVerified("PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests")]
    public xAxis xAxis { get => xAxisField; set => xAxisField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests")]
    private yAxis yAxisField;

    [MirrorVerified("PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests")]
    public yAxis yAxis { get => yAxisField; set => yAxisField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests")]
    private zAxis zAxisField;

    [MirrorVerified("PtcPersistentDataObjectsaxes3TypeMechanicalDifferentialTests")]
    public zAxis zAxis { get => zAxisField; set => zAxisField = value; }
}
