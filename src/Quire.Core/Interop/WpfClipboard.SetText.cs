#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
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
