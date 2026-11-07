#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Core.Serialization;

public partial class AnnotationTypeList
{
    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    private readonly List<MC14ConversionAnnotationType> _annotationsList;

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public AnnotationTypeList()
    {
        _annotationsList = new List<MC14ConversionAnnotationType>();
    }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public AnnotationTypeList(AnnotationTypeList source, bool isFrozen = false)
    {
        _annotationsList = source._annotationsList.ToList();
        IsFrozen = isFrozen;
    }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public string[] ToStrings()
    {
        return this.Collect((MC14ConversionAnnotationType item) => item.ToString()).ToArray();
    }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public MC14ConversionAnnotationType this[int idx] => _annotationsList[idx];

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public void AddOnce(MC14ConversionAnnotationType type)
    {
        if (DebugTools.DevAssert(!IsFrozen, null).Failed)
        {
            return;
        }
        if (!_annotationsList.Contains(type))
        {
            _annotationsList.Add(type);
        }
    }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public bool Contains(string typeString)
    {
        MC14ConversionAnnotationType item = (MC14ConversionAnnotationType)Enum.Parse(typeof(MC14ConversionAnnotationType), typeString);
        return _annotationsList.Contains(item);
    }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public void Add(MC14ConversionAnnotationType item)
    {
        if (DebugTools.DevAssert(!IsFrozen, null).Failed)
        {
            return;
        }
        _annotationsList.Add(item);
    }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public void Clear()
    {
        if (DebugTools.DevAssert(!IsFrozen, null).Failed)
        {
            return;
        }
        _annotationsList.Clear();
    }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public bool Contains(MC14ConversionAnnotationType item) => _annotationsList.Contains(item);

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public void CopyTo(MC14ConversionAnnotationType[] array, int arrayIndex) => _annotationsList.CopyTo(array, arrayIndex);

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public bool Remove(MC14ConversionAnnotationType item)
    {
        if (DebugTools.DevAssert(!IsFrozen, null).Passed)
        {
            return _annotationsList.Remove(item);
        }
        return false;
    }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public int Count => _annotationsList.Count;

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public bool IsReadOnly => IsFrozen;

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public void Freeze() => IsFrozen = true;

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public bool IsFrozen { get; private set; }

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    public IEnumerator<MC14ConversionAnnotationType> GetEnumerator() => _annotationsList.GetEnumerator();

    [MirrorVerified("PtcControlsCoreSerializationAnnotationTypeListMechanicalDifferentialTests")]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
