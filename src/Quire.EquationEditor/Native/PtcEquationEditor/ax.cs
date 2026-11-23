#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Parsing;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ax
{
    [MirrorVerified("Release IL")]
    public NavigatorState b(Direction direction, MathNode mathNode, DelegateFunction0<MathNode> callback, Nullable<Fixity> fixity) => a(direction, mathNode, callback, fixity);

    [MirrorVerified("Release IL")]
    public NavigatorState b(Direction direction, MathNode mathNode, MathNode mathNodeArg, Nullable<Fixity> fixity) => a(direction, mathNode, mathNodeArg, fixity);

    [MirrorVerified("Release IL")]
    public NavigatorState c(MathNode mathNode, int index, string text) => b(mathNode, index, index, text);

    [MirrorVerified("Release IL")]
    public NavigatorState a(MathNode mathNode, int index, string text, CaretInsertionTypes caretInsertionTypes) => a(mathNode, index, index, text, caretInsertionTypes);

    [MirrorVerified("Release IL")]
    public NavigatorState a(MathNode mathNode, int index, FormattedString formattedString) => b(mathNode, index, index, formattedString);

    [MirrorVerified("Release IL")]
    public NavigatorState a(MathNode mathNode, int index, string text) => a(mathNode, index, index, text);

    [MirrorVerified("Release IL")]
    public NavigatorState a(MathNode mathNode, string text) => a(false, mathNode, text);

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
