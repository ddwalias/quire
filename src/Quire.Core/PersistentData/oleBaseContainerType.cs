#nullable disable
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial class oleBaseContainerType
{
    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    protected oleBaseContainerType() { }

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    private SizeType oleViewportSizeField;

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    public SizeType OleViewportSize { get => oleViewportSizeField; set => oleViewportSizeField = value; }

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    public string StorageFileName { private get; set; }

    [MirrorVerified("PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests")]
    public IS11NProvider S11NProvider { get; set; }
}
