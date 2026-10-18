#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class trace2DType
{
    [MirrorVerified("PtcPersistentDataObjectstrace2DTypeMechanicalDifferentialTests")]
    public trace2DType() { }

    [MirrorVerified("PtcPersistentDataObjectstrace2DTypeMechanicalDifferentialTests")]
    private string traceLabelField;

    [MirrorVerified("PtcPersistentDataObjectstrace2DTypeMechanicalDifferentialTests")]
    public string traceLabel { get => traceLabelField; set => traceLabelField = value; }
}
