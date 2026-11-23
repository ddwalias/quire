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
    private int b(XmlNode A_0) => a(A_0, "power-denominator", 1);

    [MirrorVerified("Release IL")]
    private int a(XmlNode A_0) => a(A_0, "power-numerator", 1);
}
