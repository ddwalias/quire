#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public partial class ComboBoxInputControlData
{
    [MirrorVerified("Release IL")]
    public override object Clone() => new ComboBoxInputControlData(this);

    [MirrorVerified("Release IL")]
    private readonly int c;

    [MirrorVerified("Release IL")]
    public int Cols => c;

    [MirrorVerified("Release IL")]
    private readonly List<MathNode> f;

    [MirrorVerified("Release IL")]
    public List<MathNode> Values => f;

    [MirrorVerified("Release IL")]
    private readonly IList<MathNode> g;

    [MirrorVerified("Release IL")]
    public IList<MathNode> Units => g;

    [MirrorVerified("Release IL")]
    private readonly string h;

    [MirrorVerified("Release IL")]
    public string TableTitle => h;

    [MirrorVerified("Release IL")]
    public int SelectedValueRow => SelectedRowId;

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }

        [MirrorVerified("Release IL")]
        internal ValueTuple<MathNode, MathNode> a(MathNode mathNode, MathNode mathNodeArg) => new ValueTuple<MathNode, MathNode>(mathNode, mathNodeArg);

        [MirrorVerified("Release IL")]
        internal string i(MathNode mathNode) => MathNodeToStringTransformer.TransformToTitleViewString(mathNode);

        [MirrorVerified("Release IL")]
        internal string h(MathNode mathNode) => MathNodeToStringTransformer.TransformToValueViewString(mathNode);

        [MirrorVerified("Release IL")]
        internal string g(MathNode mathNode) => MathNodeToStringTransformer.TransformToUnitViewString(mathNode);

        [MirrorVerified("Release IL")]
        internal void f(MathNode mathNode) => mathNode.MakeReadOnly();

        [MirrorVerified("Release IL")]
        internal void e(MathNode mathNode) => mathNode.MakeReadOnly();

        [MirrorVerified("Release IL")]
        internal void d(MathNode mathNode) => mathNode.MakeReadOnly();

        [MirrorVerified("Release IL")]
        internal void c(MathNode mathNode) => mathNode.MakeEditable();

        [MirrorVerified("Release IL")]
        internal void b(MathNode mathNode) => mathNode.MakeEditable();

        [MirrorVerified("Release IL")]
        internal void a(MathNode mathNode) => mathNode.MakeEditable();
    }
}
