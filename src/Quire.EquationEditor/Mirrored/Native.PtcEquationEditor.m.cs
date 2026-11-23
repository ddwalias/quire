using System;

using System.Text.RegularExpressions;
using Quire.Controls.Core;
using Quire.EquationEditor;
using Quire.EquationEditor.Equation;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Scoping;
using Quire.Mirror;

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

    public m(EquationModel model)
        : base(model)
    {
    }

    public override void a2()
    {
        NavigatorState state = aa().a();
        c(state);
        a1();
        av().t();
    }

    private FormattedString a(string text, string find, string replacement, bool matchCase, out int index)
    {
        Regex regex = new Regex(Regex.Escape(find), matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
        Match match = text != null ? regex.Match(text) : null;
        if (match != null && match.Success)
        {
            index = match.Index;
            return regex.Replace(text, replacement, 1);
        }

        index = -1;
        return string.Empty;
    }

    public override bool a3(FindReplaceQuery query)
    {
        bool hasSelection = aa().t() != aa().s();
        if (query.ValueToReplaceWith.IsNullOrEmpty() || (hasSelection && query.ValueToFind.IsNotEmpty()))
        {
            query.Status = FindReplaceStatus.ExecuteReplaceInScopeFailed;
        }
        else if (DebugTools.DevAssert(query.Status.IsEmpty(), null).Failed)
        {
            query.Status = FindReplaceStatus.Empty;
        }

        return query.Status.IsEmpty();
    }

    public override bool x(FormattedString text)
    {
        NavigatorState state = al().a(new LinearSegment(aa().t(), aa().s()), text);
        if (state.Equals(NavigatorState.Empty))
        {
            return false;
        }

        b(state);
        c(state);
        a1();
        av().t();
        return true;
    }

    public override void ad()
    {
        if (aa().m())
        {
            a1();
            av().ad();
            return;
        }

        ak state = aa().x();
        base.ad();
        aa().a(state);
        h();
    }

    public override void ae()
    {
        a1();
        av().ae();
        b();
    }

    public override void af()
    {
        a1();
        av().af();
        b();
    }

    public override void ag(MathNode node, Direction direction)
    {
        a1();
        av().ag(node, direction);
    }

    public override void s()
    {
        if (aa().v() == Direction.Left)
        {
            aa().e(Direction.Right);
        }
        else
        {
            aa().e(Direction.Left);
        }

        c();
    }

    private void a(Delegate0 action)
    {
        bool wasScoping = aa().r();
        action();
        if (!aa().r() && !wasScoping)
        {
            a1();
        }
        else if (!aa().r())
        {
            a2();
        }
        else
        {
            c();
        }
    }

    public override void am()
    {
        a(aa().g);
    }

    public override void an()
    {
        a(aa().h);
    }

    public override void ao()
    {
        NavigatorState state = aa().q();
        if (!aa().l())
        {
            c(state);
            a1();
        }

        av().t();
    }

    public override void a8()
    {
        aa().k();
        if (!aa().r())
        {
            c(aa().q());
            a1();
        }

        av().t();
    }

    public override void k(Direction direction)
    {
        Direction current = ax();
        a2();
        if (current == direction)
        {
            av().k(direction);
        }
    }

    public override void q(Direction direction)
    {
        if (ax() == DirectionOps.Opposite(direction))
        {
            r(direction);
            return;
        }

        a2();
        av().q(direction);
    }

    public override void t()
    {
        if (av() != this)
        {
            throw new InvalidOperationException("Logic Error: Model is not in ScopingMode and we're notifying that we are in a new ScopeState");
        }

        c();
    }

    public override void a9()
    {
        a1();
        b();
    }

    protected new DirectionalPair<MathNode, MathNode> a()
    {
        return new DirectionalPair<MathNode, MathNode>
        {
            Left = aa().n().GetFirstScopedNode,
            Right = aa().n().GetLastScopedNode,
        };
    }

    public override void al(MathNode node, Direction first, Direction second)
    {
        a1();
        av().al(node, first, second);
        av().t();
    }

    public override void ap()
    {
        h();
        aa().j();
        c();
    }

    protected override bool aq(Direction direction, MathNode node)
    {
        if (ax() == Direction.Left && aa().t().ContainerRoot.Parent == node)
        {
            return bq.b(aa().t());
        }

        return false;
    }

    protected override Direction ar(NavigatorState state)
    {
        Direction result = Direction.Indeterminate;
        if (state.Current.CarriesValue)
        {
            if (state.SelectionStart == 0 || state.CaretSide == Direction.Left)
            {
                result = Direction.Right;
            }
            else
            {
                result = Direction.Left;
            }
        }
        else if (state.CaretSide != Direction.Indeterminate)
        {
            result = DirectionOps.Opposite(state.CaretSide);
        }
        else if (state.Current.IsPlaceholder)
        {
            result = DirectionOps.Opposite(ax());
        }

        return result;
    }

    public override bool @as(IdLabels labels)
    {
        if (aa().t() != aa().s())
        {
            return false;
        }

        bool flag = base.@as(labels);
        if (flag)
        {
            ak state = aa().x();
            b(f());
            aa().a(state);
            h();
            t();
        }

        return flag;
    }

    public override bool at()
    {
        if (aa().t() != aa().s())
        {
            return false;
        }

        bool flag = base.at();
        if (flag)
        {
            ak state = aa().x();
            b(f());
            aa().a(state);
            h();
            t();
        }

        return flag;
    }

    public override bool au()
    {
        a2();
        return av().au();
    }

    private void a(bool notify)
    {
        if (!notify)
        {
            av().ao();
            av().t();
        }
    }

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }

        [MirrorVerified("Release IL")]
        internal bool a(MathNode A_0) => A_0.IsEvaluatable;
    }
}
