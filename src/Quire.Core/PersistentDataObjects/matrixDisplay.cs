#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class matrixDisplay
{
    [MirrorVerified("PtcPersistentDataObjectsmatrixDisplayMechanicalDifferentialTests")]
    public matrixDisplay() { }

    [MirrorVerified("PtcPersistentDataObjectsmatrixDisplayMechanicalDifferentialTests")]
    private string offsetField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixDisplayMechanicalDifferentialTests")]
    public string offset { get => offsetField; set => offsetField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmatrixDisplayMechanicalDifferentialTests")]
    private string sizeField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixDisplayMechanicalDifferentialTests")]
    public string size { get => sizeField; set => sizeField = value; }
}
