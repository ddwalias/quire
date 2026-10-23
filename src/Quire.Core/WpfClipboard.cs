#nullable disable
using System.Windows;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
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
    internal static readonly string _richTextEmbeddedItemsInfo;

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasTextWithEmbeddedItemsInfo() => Clipboard.ContainsData(_richTextEmbeddedItemsInfo);

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public WpfClipboard() { }

    [MirrorVerified("Release IL")]
    public static System.IDisposable Override(IClipboard clipboard)
    {
        System.IDisposable clipboardOverride = new ClipboardOverride(_current);
        _current = clipboard;
        return clipboardOverride;
    }

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
