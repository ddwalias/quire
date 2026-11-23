#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public partial class XmlNodeToMathNodeTransform
{
    partial class c_
    {
        [MirrorVerified("Release IL")]
        public c_() { }

        [MirrorVerified("Release IL")]
        internal string a(XmlElement xmlElement) => xmlElement.InnerText;
    }

    partial class d_
    {
        [MirrorVerified("Release IL")]
        public d_() { }
    }
}
