#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public void Clear() => Clipboard.Clear();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public string GetText() => Clipboard.GetText();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    private static readonly string _mathNodeSExpression;

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public static bool CopySExpressionToDataObject(IDataObject dataObject, string sExpression)
    {
        dataObject.SetData(_mathNodeSExpression, sExpression);
        return true;
    }

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public static bool CopyRtfStringToDataObject(DataObject dataObject, string rtfString)
    {
        dataObject.SetData(DataFormats.Rtf, rtfString);
        return true;
    }

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasText() => Clipboard.ContainsText();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasImage() => Clipboard.ContainsImage();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    private static readonly string _primeXamlStreamCompressed;

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasPrimeObject() => Clipboard.ContainsData(_primeXamlStreamCompressed);

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    private static readonly string _xmlSpreadsheetDataType;

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasXMLSpreadsheet() => Clipboard.ContainsData(_xmlSpreadsheetDataType);

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasSExpression() => Clipboard.ContainsData(_mathNodeSExpression);

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    internal static readonly string _richTextEmbeddedItemsInfo;

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasTextWithEmbeddedItemsInfo() => Clipboard.ContainsData(_richTextEmbeddedItemsInfo);

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public WpfClipboard() { }

    partial class ClipboardOverride
    {
        [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
        private readonly IClipboard _oldClipboard;

        [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
        public ClipboardOverride(IClipboard oldClipboard)
        {
            _oldClipboard = oldClipboard;
        }

        [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
        public void Dispose() => _current = _oldClipboard;
    }
}
