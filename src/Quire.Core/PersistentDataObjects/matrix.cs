#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class matrix
{
    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    public matrix() { }

    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    private matrixDisplay displayField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    public matrixDisplay display { get => displayField; set => displayField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    private object[] itemsField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    public object[] Items { get => itemsField; set => itemsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    private string rowsField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    public string rows { get => rowsField; set => rowsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    private string colsField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixMechanicalDifferentialTests")]
    public string cols { get => colsField; set => colsField = value; }
}
