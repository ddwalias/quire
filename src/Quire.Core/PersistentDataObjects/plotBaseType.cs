#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class plotBaseType
{
    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    public plotBaseType() { }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    private object titleField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    public object title { get => titleField; set => titleField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    private plotBaseTypeLegend legendField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    public plotBaseTypeLegend legend { get => legendField; set => legendField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    private trace2DType[] tracesField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    public trace2DType[] traces { get => tracesField; set => tracesField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    private SizeType graphsizeField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    public SizeType graphsize { get => graphsizeField; set => graphsizeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    public IEnumerable<string> MC14ConversionLossAnnotations => MC14ConversionLossAnnotationsLocal;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests")]
    public List<string> MC14ConversionLossAnnotationsLocal { get; set; }
}
