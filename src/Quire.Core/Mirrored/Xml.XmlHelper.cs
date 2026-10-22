#nullable disable
#pragma warning disable

namespace Quire.Xml;

static partial class XmlHelper
{
    [global::Quire.Mirror.MirrorVerified("Release IL")]
    public static global::System.Xml.XmlAttribute AddAttribute(global::System.Xml.XmlElement node, string name, string value)
    {
        global::System.Xml.XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
        attribute.Value = value;
        node.Attributes.Append(attribute);
        return attribute;
    }

    [global::Quire.Mirror.MirrorVerified("Release IL")]
    public static global::System.Xml.XmlElement AddChild(global::System.Xml.XmlElement node, string name, string innerText, bool nonPrintableExpected)
    {
        global::System.Xml.XmlElement element = node.OwnerDocument.CreateElement(name);
        element.InnerText = ReplaceNonPrintableCharacters(innerText);
        if (!nonPrintableExpected)
        {
            global::Quire.DebugTools.DevAssert(innerText.Length == element.InnerText.Length, null);
        }
        node.AppendChild(element);
        return element;
    }

    [global::Quire.Mirror.MirrorVerified("Release IL")]
    public static string ReplaceNonPrintableCharacters(string text)
    {
        if (global::Quire.EnumerableExtensions.IsNullOrEmpty<char>(text))
        {
            return text;
        }
        return global::System.Text.RegularExpressions.Regex.Replace(text, "[\0-\b]|[\v-\f]|[\u000e-\u001f]", match =>
        {
            string value = match.Groups[0].Value;
            return string.Format("&#x{0:x4};", (int)value[0]);
        });
    }

    [global::Quire.Mirror.MirrorAnalyzed("Ptc.Xml.XmlHelper")]
    public static object CreateXamlCopy(object source)
    {
        object result;
        using (global::Quire.MindTouch.IO.ChunkedMemoryStream chunkedMemoryStream = new global::Quire.MindTouch.IO.ChunkedMemoryStream())
        {
            using (global::System.IO.StreamWriter streamWriter = CreateStreamWriterForXamlCopy(chunkedMemoryStream, leaveOpen: true))
            {
                global::System.Windows.Markup.XamlWriter.Save(source, streamWriter);
                streamWriter.Flush();
            }
            chunkedMemoryStream.Position = 0L;
            result = global::System.Windows.Markup.XamlReader.Load(chunkedMemoryStream);
        }
        return result;
    }

    // Placeholder stub, declared here so ReplaceNonPrintableCharacters keeps Mathcad's member ordinal. Mathcad's
    // "new XPathDocument(stream) ?? throw" leaves the value on the stack at the throw; .NET 10 Roslyn always pops it,
    // so this body cannot match Mathcad's IL.
    public static global::System.Collections.Generic.List<string> GetNodeValueFromXml(string xmlPath, string xPathExpr) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
