#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class matrixFormatType
{
    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    private matrixFormatTypeNestedMatrixFormat[] nestedMatrixFormatsField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    public matrixFormatTypeNestedMatrixFormat[] nestedMatrixFormats { get => nestedMatrixFormatsField; set => nestedMatrixFormatsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    private bool expandnestedarraysField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    public bool expandnestedarrays { get => expandnestedarraysField; set => expandnestedarraysField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    private string sizeField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    public string size { get => sizeField; set => sizeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    private string offsetField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    public string offset { get => offsetField; set => offsetField = value; }

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    private bool showindicesField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeMechanicalDifferentialTests")]
    public bool showindices { get => showindicesField; set => showindicesField = value; }

    [MirrorVerified("Release IL")]
    public matrixFormatType()
    {
        expandnestedarraysField = false;
        showindicesField = false;
    }
}
