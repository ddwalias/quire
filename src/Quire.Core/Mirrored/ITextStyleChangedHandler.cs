#nullable disable
#pragma warning disable

namespace Quire;

partial interface ITextStyleChangedHandler
{
    event global::Quire.Delegate1<global::System.Collections.Generic.ICollection<global::Quire.TextStyleData>> StyleChanged;
    event global::Quire.Delegate1<global::Quire.Core.TextFormat> StyleCreateRequest;
    void UpdateStyle(global::System.Collections.Generic.ICollection<global::Quire.TextStyleData> textStyles);
    void CreateNewStyle(global::Quire.Core.TextFormat textFormat);
}
