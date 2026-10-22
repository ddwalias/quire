#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial interface IPersistentDataProvider
{
    global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serialization.IDeS11NProvider d11NProvider, object data) ;
    object GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider) ;
}
