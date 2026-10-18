#nullable disable
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Core.Serialization;

public partial class AnnotationTypeList
{
    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    private readonly List<MC14ConversionAnnotationType> _annotationsList;

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public bool Contains(MC14ConversionAnnotationType item) => _annotationsList.Contains(item);

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public void CopyTo(MC14ConversionAnnotationType[] array, int arrayIndex) => _annotationsList.CopyTo(array, arrayIndex);

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public void Freeze() => IsFrozen = true;

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public MC14ConversionAnnotationType this[int idx] => _annotationsList[idx];

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public int Count => _annotationsList.Count;

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public bool IsReadOnly => IsFrozen;

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public bool IsFrozen { get; private set; }
}
