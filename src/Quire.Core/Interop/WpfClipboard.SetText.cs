#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    private static void CreateBitmapFromDib(System.IO.Stream dib, System.IO.Stream outStream)
    {
        int dibHeaderSize = new System.IO.BinaryReader(dib).ReadInt32();
        int dataSize = (int)dib.Length - dibHeaderSize;
        int fileSize = 14 + dibHeaderSize + dataSize;
        System.IO.BinaryWriter writer = new System.IO.BinaryWriter(outStream);
        writer.Write((byte)66);
        writer.Write((byte)77);
        writer.Write(fileSize);
        writer.Write(0);
        writer.Write(14 + dibHeaderSize);
        dib.Position = 0L;
        Quire.IO.StreamUtils.CopyStream(dib, outStream);
    }

    [MirrorVerified("Release IL")]
    public static bool CopyItemAsImageToDataObject(System.Windows.DataObject dataObject, object whiteboardItem)
    {
        System.Windows.FrameworkElement element = whiteboardItem as System.Windows.FrameworkElement;
        if (element == null)
        {
            return false;
        }
        System.Windows.Media.Imaging.BitmapSource bitmapSource;
        bool result;
        try
        {
            bitmapSource = Quire.Wpf.ImageUtils.CreatePngFromElement(element);
            dataObject.SetImage(bitmapSource);
            result = true;
            goto Success;
        }
        catch
        {
        }
        return false;
    Success:
        return result;
    }

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public void Clear() => System.Windows.Clipboard.Clear();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public string GetText() => System.Windows.Clipboard.GetText();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasText() => System.Windows.Clipboard.ContainsText();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public string GetSExpression() => System.Windows.Clipboard.GetData(_mathNodeSExpression) as string;

    [MirrorVerified("Release IL")]
    public void SetText(string text) => SafeClipboardAction(() => System.Windows.Clipboard.SetText(text));

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasSExpression() => System.Windows.Clipboard.ContainsData(_mathNodeSExpression);

    [MirrorVerified("Release IL")]
    public static void SetDataObjectToClipboard(System.Windows.IDataObject dataObject, bool copy = false) => SafeClipboardAction(() => System.Windows.Clipboard.SetDataObject(dataObject, copy));

}
