#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial interface IHasPreSerialize
{
    void PreSerialize() ;
}
