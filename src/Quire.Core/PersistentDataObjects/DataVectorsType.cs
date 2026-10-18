#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class DataVectorsType
{
    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    public DataVectorsType() { }

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    private string countField;

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    public string Count { get => countField; set => countField = value; }

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    private ValuesTypeEnum valuesTypeField;

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    public ValuesTypeEnum ValuesType { get => valuesTypeField; set => valuesTypeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    private string vectorLengthField;

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    public string VectorLength { get => vectorLengthField; set => vectorLengthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    private VectorsTypeEnum vectorsTypeField;

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    public VectorsTypeEnum VectorsType { get => vectorsTypeField; set => vectorsTypeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    private string valueField;

    [MirrorVerified("PtcPersistentDataObjectsDataVectorsTypeMechanicalDifferentialTests")]
    public string Value { get => valueField; set => valueField = value; }
}
