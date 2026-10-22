#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial interface IRegionPersistentData
{
    global::System.Windows.UIElement Control { get; }
    global::System.Windows.Point Location { get; set; }
    global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> Children { set; }
    global::System.Exception DeserializationException { set; }
    global::System.Collections.Generic.IEnumerable<global::System.Exception> AllDeserializationExceptions { get; }
    bool HasDeserializationErrors { get; }
    global::Quire.PersistentDataObjects.IRegionType RegionData { get; }
    void Deserialize(global::Quire.PersistentDataObjects.IRegionType regionData) ;
    void DeserializeEpilog() ;
}
