using Quire.EquationEditor;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;

namespace Quire.Native.PtcEquationEditor;

internal partial class n
{
    public override EquationEditModeEnum e() => EquationEditModeEnum.Typeover;

    public override NavigatorState f() => ab().NavState;

    public override string ba() => ab().SExpression;

    public override System.Collections.Generic.IList<MathNode> bb() => ab().TypeoverNodes;

    protected override MathNode g() => ab().LeftNode;

    public override bool a0() => false;

    public override void bc() => c();

    public new void c()
    {
        a(ab().NavState);
    }

    public override void k(Direction A_0) => c();

    public override void q(Direction A_0) => c();

    public override void am() => c();

    public override void an() => c();

    public override void ao() => c();

    public override void a9() => c();

    public override void ap() => c();
}
