using System;
using System.Runtime.CompilerServices;
using Quire.EquationEditor;
using Quire.EquationEditor.Equation;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Scoping;

namespace Quire.Native.PtcEquationEditor;

internal partial class m
{
    public override NavigatorState f() => aa().q();

    public override Direction ax() => aa().v();

    public override string ay() => aa().w();

    public override EquationEditModeEnum e() => EquationEditModeEnum.Scope;

    public override ScopedNodes az() => aa().n();

    protected override MathNode g() => aa().t();

    public override bool a0()
    {
        if (aa().t() != aa().s())
        {
            return false;
        }

        return base.a0();
    }

    public override void a1()
    {
        if (ag().Equals(NavigatorState.Empty))
        {
            c(aa().q());
        }

        aa().i();
        base.a1();
    }

    public override void i()
    {
        aa().i();
        base.i();
    }

    public override bool a5() => true;

    public override bool a7()
    {
        if (a5())
        {
            a6();
            return j(Direction.Left);
        }

        return false;
    }

    public override bool n() => u("GROUPING");

    public override bool w(MathNode A_0) => false;
}
