#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public partial class XmcdToMathNodeTransform
{
    [MirrorVerified("Release IL")]
    public XmcdToMathNodeTransform() : this(null, null) { }

    [MirrorVerified("Release IL")]
    public MathNode Transform(XmlNode node) => g(node);

    [MirrorVerified("Release IL")]
    private int b(XmlNode xmlNode) => a(xmlNode, "power-denominator", 1);

    [MirrorVerified("Release IL")]
    private int a(XmlNode xmlNode) => a(xmlNode, "power-numerator", 1);
}
