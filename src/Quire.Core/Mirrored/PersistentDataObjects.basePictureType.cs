#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial class basePictureType
{
    public string itemidref
    {
        get
        {
            if (global::System.String.IsNullOrEmpty(itemidrefField))
            {
                itemidrefField = global::Quire.Serialization.IS11NProviderExtensions.PackImageExt(S11NProvider, default(global::Quire.Wpf.ImagePackagingInformation));
            }
            return itemidrefField;
        }
        set => itemidrefField = value;
    }
}
