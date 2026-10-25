#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire;

public static partial class DataObjectExtensions
{
    [MirrorVerified("PtcDataObjectExtensionsMechanicalDifferentialTests")]
    public static bool HasTextWithEmbeddedItemsInfo(this IDataObject dataObject) => dataObject.GetDataPresent(WpfClipboard._richTextEmbeddedItemsInfo);

    [MirrorVerified("Release IL")]
    public static void SetTextWithEmbeddedItemsInfo(this IDataObject dataObject, RichTextEmbeddedItemsInfo data)
    {
        dataObject.SetData(WpfClipboard._richTextEmbeddedItemsInfo, (int)data);
    }

    public static global::System.Nullable<RichTextEmbeddedItemsInfo> GetTextWithEmbeddedItemsInfo(this IDataObject dataObject)
    {
        if (!dataObject.HasTextWithEmbeddedItemsInfo())
        {
            return null;
        }

        object data;
        try
        {
            data = dataObject.GetData(WpfClipboard._richTextEmbeddedItemsInfo);
        }
        catch (global::System.NotSupportedException)
        {
            return null;
        }

        if (data is global::System.Exception)
        {
            return null;
        }

        object value = data;
        return value switch
        {
            RichTextEmbeddedItemsInfo embeddedItemsInfo => embeddedItemsInfo,
            int integerValue => (RichTextEmbeddedItemsInfo)integerValue,
            string text when global::System.Int32.TryParse(text, out int parsedValue) =>
                (RichTextEmbeddedItemsInfo)parsedValue,
            _ => null
        };
    }
}
