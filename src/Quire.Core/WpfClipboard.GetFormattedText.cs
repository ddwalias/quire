#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    public string GetFormattedText(out string dataFormat, out bool isPlainText)
    {
        isPlainText = false;
        if (System.Windows.Clipboard.ContainsText((System.Windows.TextDataFormat)5) && HasTextWithEmbeddedItemsInfo())
        {
            dataFormat = System.Windows.DataFormats.Xaml;
        }
        else if (System.Windows.Clipboard.ContainsText((System.Windows.TextDataFormat)2))
        {
            dataFormat = System.Windows.DataFormats.Rtf;
        }
        else
        {
            isPlainText = true;
            dataFormat = System.Windows.DataFormats.UnicodeText;
        }
        if (!isPlainText)
        {
            return System.Windows.Clipboard.GetData(dataFormat) as string;
        }
        return GetText();
    }
}
