#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class oleBaseContainerType
{
    public string itemidref
    {
        get
        {
            if (global::System.String.IsNullOrEmpty(itemidrefField))
            {
                itemidrefField = global::Quire.Serialization.IS11NProviderExtensions.PackExternalObjectExt(S11NProvider, StorageFileName, null, ExternalObjectType);
            }
            return itemidrefField;
        }
        set => itemidrefField = value;
    }
    protected global::System.Windows.Size GetOleViewportSize()
    {
        if (OleViewportSize != null)
        {
            return new global::System.Windows.Size(OleViewportSize.width, OleViewportSize.height);
        }
        return global::System.Windows.Size.Empty;
    }
}
