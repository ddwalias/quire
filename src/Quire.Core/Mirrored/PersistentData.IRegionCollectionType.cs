#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial interface IRegionCollectionType
{
    global::System.Collections.Generic.IEnumerable<global::Quire.PersistentDataObjects.IRegionType> Regions { get; set; }
}
