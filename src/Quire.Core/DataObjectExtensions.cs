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
}
