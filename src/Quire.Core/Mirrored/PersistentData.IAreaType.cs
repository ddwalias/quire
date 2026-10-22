#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial interface IAreaType
{
    bool IsCollapsed { get; set; }
    bool IsBorderVisible { get; set; }
    global::Quire.ProtectionData ProtectionDataWrapper { get; set; }
}
