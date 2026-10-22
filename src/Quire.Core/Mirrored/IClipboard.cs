#nullable disable
#pragma warning disable

namespace Quire;

partial interface IClipboard
{
    void Clear();
    string GetText();
    void SetText(string text);
    bool HasText();
    global::System.Windows.Media.Imaging.BitmapSource GetImage();
    bool HasImage();
    object GetPrimeObject();
    bool HasPrimeObject();
    global::System.Xml.XmlDocument GetXMLSpreadsheet();
    bool HasXMLSpreadsheet();
    string GetFormattedText(out string dataFormat, out bool isPlainText);
    global::System.IO.Stream GetPrimeXamlStream();
    void SetSExpression(string sExpression);
    bool HasSExpression();
    string GetSExpression();
}
