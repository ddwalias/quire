#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    public System.Xml.XmlDocument GetXMLSpreadsheet()
    {
        System.IO.Stream stream = System.Windows.Clipboard.GetData(_xmlSpreadsheetDataType) as System.IO.Stream;
        if (stream == null)
        {
            return null;
        }
        System.Xml.XmlDocument document = new System.Xml.XmlDocument();
        document.Load(stream);
        return document;
    }
}
