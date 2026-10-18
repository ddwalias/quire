#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class trace3DType
{
    [MirrorVerified("PtcPersistentDataObjectstrace3DTypeMechanicalDifferentialTests")]
    public trace3DType() { }

    [MirrorVerified("PtcPersistentDataObjectstrace3DTypeMechanicalDifferentialTests")]
    private string surfaceFillField;

    [MirrorVerified("PtcPersistentDataObjectstrace3DTypeMechanicalDifferentialTests")]
    public string SurfaceFill { get => surfaceFillField; set => surfaceFillField = value; }
}
