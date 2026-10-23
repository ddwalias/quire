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

    [MirrorVerified("Release IL")]
    public void SetText(string text) => SafeClipboardAction(() => System.Windows.Clipboard.SetText(text));
}
