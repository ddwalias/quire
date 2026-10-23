#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
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
