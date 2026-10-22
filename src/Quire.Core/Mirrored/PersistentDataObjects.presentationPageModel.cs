#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial class presentationPageModel
{
    public global::System.Nullable<global::System.Windows.Thickness> PageMargin
    {
        get
        {
            if (!global::System.String.IsNullOrEmpty(pagemargin))
            {
                return (global::System.Nullable<global::System.Windows.Thickness>)_thicknessConverter.ConvertFromInvariantString(pagemargin);
            }
            return default(global::System.Nullable<global::System.Windows.Thickness>);
        }
        set
        {
            if (value.HasValue)
            {
                pagemargin = _thicknessConverter.ConvertToInvariantString(value);
            }
        }
    }
}
