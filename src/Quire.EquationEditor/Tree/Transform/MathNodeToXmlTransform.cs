#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public partial class MathNodeToXmlTransform
{
    [MirrorVerified("Release IL")]
    private static XmlElement a(XmlDocument xmlDocument, string text) => xmlDocument.CreateElement("ml", text, "http://schemas.mathsoft.com/math50");

    [MirrorVerified("Release IL")]
    public static XmlNode TransformPlaceholderNode(MathNodeToXmlTransform transformer, XmlDocument document, MathNode node) => a(document, "placeholder");

    [MirrorVerified("Release IL")]
    private static XmlElement a(XmlDocument xmlDocument) => a(xmlDocument, "unitedValue");

    [MirrorVerified("Release IL")]
    public static XmlNode TransformScaleNode(MathNodeToXmlTransform transformer, XmlDocument document, MathNode node) => TransformOperatorNode(transformer, document, node);

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }
    }

    partial class c_
    {
        [MirrorVerified("Release IL")]
        public c_() { }
    }

    partial class d_
    {
        [MirrorVerified("Release IL")]
        public d_() { }
    }

    partial class e_
    {
        [MirrorVerified("Release IL")]
        public e_() { }
    }

    partial class f_
    {
        [MirrorVerified("Release IL")]
        public f_() { }
    }

    partial class g_
    {
        [MirrorVerified("Release IL")]
        public g_() { }
    }

    partial class h_
    {
        [MirrorVerified("Release IL")]
        public h_() { }
    }

    partial class i
    {
        [MirrorVerified("Release IL")]
        public i() { }
    }

    partial class j
    {
        [MirrorVerified("Release IL")]
        public j() { }
    }

    partial class k
    {
        [MirrorVerified("Release IL")]
        public k() { }
    }

    partial class l
    {
        [MirrorVerified("Release IL")]
        public l() { }
    }
}
