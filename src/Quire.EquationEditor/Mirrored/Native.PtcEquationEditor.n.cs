using System;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Quire.Controls.Core;
using Quire.Core;
using Quire.EquationEditor;
using Quire.EquationEditor.Equation;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Scoping;
using Quire.Matrices;
using Quire.Mirror;

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

    public n(EquationModel model)
        : base(model)
    {
    }

    private void a(NavigatorState state)
    {
        c(state);
        a((TypeoverState)null);
        base.a1();
        av().t();
    }

    public override void a1()
    {
        if (ag().Equals(NavigatorState.Empty))
        {
            c(aa().q());
        }

        a((TypeoverState)null);
        base.a1();
    }

    public override void al(MathNode node, Direction first, Direction second)
    {
        if (!ab().AreMatrixCellsInTypeover())
        {
            c();
            return;
        }

        c();
        av().al(node, first, second);
    }

    public override bool j(Direction direction)
    {
        if (!ab().IsMatrixInTypeover() && ab().AreMatrixCellsInTypeover())
        {
            return b();
        }

        NavigatorState state = al().a(ab());
        if (state.Equals(NavigatorState.Empty))
        {
            return false;
        }

        b(state);
        a(state);
        return true;
    }

    [SkipLocalsInit]
    private void a(MathNode leftNode, MathNode rightNode, MathNode common, int rows, int columns, out int first, out int last)
    {
        first = leftNode.GetMatrixCellIndex(common);
        last = rightNode.GetMatrixCellIndex(common);
        // Mathcad compares the matrix cell index as a decimal; Release IL requires it.
        if ((decimal)first == -1m || (decimal)last == -1m)
        {
            if (leftNode.Equals(rightNode))
            {
                first = 0;
                last = rows * columns - 1;
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }
    }

    public override bool w(MathNode node)
    {
        return false;
    }

    public override bool x(FormattedString text)
    {
        if (ab().AreMatrixCellsInTypeover())
        {
            return false;
        }

        NavigatorState state = al().a(ab(), text);
        if (NavigatorState.Empty.Equals(state))
        {
            return false;
        }

        b(state);
        a(state);
        return true;
    }

    public override bool bd()
    {
        if (p().Equals(NavigatorState.Empty))
        {
            return false;
        }

        if (!j(Direction.Right))
        {
            c();
        }

        return true;
    }

    public override bool be()
    {
        if (k().Equals(NavigatorState.Empty))
        {
            return false;
        }

        if (!j(Direction.Right))
        {
            c();
        }

        return true;
    }

    public override void r(Direction direction)
    {
        c();
    }

    public override void s()
    {
    }

    public override void ae()
    {
        c();
    }

    public override void af()
    {
        c();
    }

    public override void ag(MathNode node, Direction direction)
    {
        a1();
        av().ag(node, direction);
    }

    public override void t()
    {
        if (av() != this)
        {
            throw new InvalidOperationException("Logic Error: Model is not in TypeoverMode and we're notifying that we are in a new TypeoverState");
        }

        base.a();
    }

    public override bool bf(Direction direction, bool flag)
    {
        c();
        return false;
    }

    public override bool bg(Direction direction)
    {
        c();
        return false;
    }

    public override bool @as(IdLabels labels)
    {
        return false;
    }

    public override bool at()
    {
        return false;
    }

    public override bool a5()
    {
        return ab().AreMatrixCellsInTypeover();
    }

    public override bool a7()
    {
        if (a5())
        {
            a6();
            return j(Direction.Left);
        }

        return false;
    }

    protected override bool av(out MathNode node)
    {
        node = null;
        return false;
    }

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }
}
