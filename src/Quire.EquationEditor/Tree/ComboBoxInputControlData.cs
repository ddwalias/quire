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
        internal ValueTuple<MathNode, MathNode> a(MathNode A_0, MathNode A_1) => new ValueTuple<MathNode, MathNode>(A_0, A_1);

        [MirrorVerified("Release IL")]
        internal string i(MathNode A_0) => MathNodeToStringTransformer.TransformToTitleViewString(A_0);

        [MirrorVerified("Release IL")]
        internal string h(MathNode A_0) => MathNodeToStringTransformer.TransformToValueViewString(A_0);

        [MirrorVerified("Release IL")]
        internal string g(MathNode A_0) => MathNodeToStringTransformer.TransformToUnitViewString(A_0);

        [MirrorVerified("Release IL")]
        internal void f(MathNode A_0) => A_0.MakeReadOnly();

        [MirrorVerified("Release IL")]
        internal void e(MathNode A_0) => A_0.MakeReadOnly();

        [MirrorVerified("Release IL")]
        internal void d(MathNode A_0) => A_0.MakeReadOnly();

        [MirrorVerified("Release IL")]
        internal void c(MathNode A_0) => A_0.MakeEditable();

        [MirrorVerified("Release IL")]
        internal void b(MathNode A_0) => A_0.MakeEditable();

        [MirrorVerified("Release IL")]
        internal void a(MathNode A_0) => A_0.MakeEditable();
    }
}
