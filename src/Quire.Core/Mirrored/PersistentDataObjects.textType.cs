#nullable disable
#pragma warning disable
using System.Linq;

namespace Quire.PersistentDataObjects;

partial class textType
{
    public global::Quire.Serialization.IS11NProvider S11NProvider { get => _s11NProvider; set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public bool SaveInline
    {
        get => _saveInline;
        set
        {
            if (value == _saveInline)
            {
                return;
            }
            if (Document != null)
            {
                global::Quire.DebugTools.DevAssert(false, null, global::System.Array.Empty<object>());
                return;
            }
            _saveInline = value;
        }
    }
    public global::Quire.Serialization.IDeS11NProvider DeS11NProvider
    {
        get => _deS11NProvider;
        set
        {
            if (_deS11NProvider != value)
            {
                _deS11NProvider = value;
                if (global::Quire.DebugTools.DevAssert(Any != null || itemidref != null, null, global::System.Array.Empty<object>()).Passed)
                {
                    if (Any != null)
                    {
                        _document = LoadFlowDocumentFromInnerXml();
                    }
                    if (itemidref != null)
                    {
                        _deS11NProvider.UnpackFlowDocument(ref _document, itemidref);
                    }
                }
            }
        }
    }
    public override global::System.Collections.Generic.IEnumerable<global::Quire.PersistentDataObjects.IRegionType> Regions { get => regions; set => regions = value?.Cast<global::Quire.PersistentDataObjects.textEmbeddedRegionType>().ToArray(); }
}
