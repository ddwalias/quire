#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Parsing;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ax
{
    [MirrorVerified("Release IL")]
    public NavigatorState b(Direction A_0, MathNode A_1, DelegateFunction0<MathNode> A_2, Nullable<Fixity> A_3) => a(A_0, A_1, A_2, A_3);

    [MirrorVerified("Release IL")]
    public NavigatorState b(Direction A_0, MathNode A_1, MathNode A_2, Nullable<Fixity> A_3) => a(A_0, A_1, A_2, A_3);

    [MirrorVerified("Release IL")]
    public NavigatorState c(MathNode A_0, int A_1, string A_2) => b(A_0, A_1, A_1, A_2);

    [MirrorVerified("Release IL")]
    public NavigatorState a(MathNode A_0, int A_1, string A_2, CaretInsertionTypes A_3) => a(A_0, A_1, A_1, A_2, A_3);

    [MirrorVerified("Release IL")]
    public NavigatorState a(MathNode A_0, int A_1, FormattedString A_2) => b(A_0, A_1, A_1, A_2);

    [MirrorVerified("Release IL")]
    public NavigatorState a(MathNode A_0, int A_1, string A_2) => a(A_0, A_1, A_1, A_2);

    [MirrorVerified("Release IL")]
    public NavigatorState a(MathNode A_0, string A_1) => a(false, A_0, A_1);

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

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

    partial class d
    {
        [MirrorVerified("Release IL")]
        public d() { }
    }

    partial class e
    {
        [MirrorVerified("Release IL")]
        public e() { }
    }

    partial class f
    {
        [MirrorVerified("Release IL")]
        public f() { }
    }
}
