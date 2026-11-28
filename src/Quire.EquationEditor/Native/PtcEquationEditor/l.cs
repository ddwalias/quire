#nullable disable
using Quire.EquationEditor;
using Quire.EquationEditor.Equation;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class l : global::Quire.EquationEditor.Equation.EquationModel.b_
{
    [MirrorVerified("Release IL")]
    public l(Quire.EquationEditor.Equation.EquationModel model) : this(model, global::Quire.Native.PtcEquationEditor.f.a_) { }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorName("b")]
    private readonly global::Quire.Native.PtcEquationEditor.f b__;
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public l(global::Quire.EquationEditor.Equation.EquationModel A_0, global::Quire.Native.PtcEquationEditor.f A_1) : base(default(global::Quire.EquationEditor.Equation.EquationModel)) { }
    [MirrorVerified("Release IL")]
    public override EquationEditModeEnum e() => EquationEditModeEnum.Normal;

    [MirrorVerified("Release IL")]
    public override NavigatorState f() => ag();

    private void b(global::Quire.EquationEditor.Tree.Navigation.NavigatorState A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [MirrorVerified("Release IL")]
    protected override MathNode g() => ag().Current;

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void h() => base.h();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void i() => base.i();
    public override bool j(global::Quire.Direction A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void k(global::Quire.Direction A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void m() => base.m();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool n() => base.n();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool o() => base.o();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool p(global::Quire.Direction A_0) => base.p(A_0);
    private global::Quire.EquationEditor.Tree.MathNode a(global::Quire.EquationEditor.Tree.Navigation.NavigatorState A_0, global::Quire.Direction A_1, ref int A_2, ref int A_3) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool b(global::Quire.Direction A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void a(global::Quire.EquationEditor.Tree.Navigation.NavigatorState A_0, global::Quire.Direction A_1) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void a(global::Quire.Direction A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void q(global::Quire.Direction A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void r(global::Quire.Direction A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [MirrorVerified("Release IL")]
    public override void s() { }

    public override void t() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool a(string A_0, global::System.Nullable<global::Quire.EquationEditor.Tree.Parsing.Fixity> A_1) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override bool u(string A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override bool v(global::Quire.DelegateFunction0<global::Quire.EquationEditor.Tree.MathNode> A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override bool w(global::Quire.EquationEditor.Tree.MathNode A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public virtual bool a(global::Quire.DelegateFunction0<global::Quire.EquationEditor.Tree.MathNode> A_0, global::System.Nullable<global::Quire.EquationEditor.Tree.Parsing.Fixity> A_1) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void a(global::Quire.EquationEditor.Tree.Navigation.NavigatorState A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool x(global::Quire.EquationEditor.Tree.FormattedString A_0) => base.x(A_0);
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool y(global::Quire.Controls.Core.InputControlTypes A_0) => base.y(A_0);
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool z(string A_0) => base.z(A_0);
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool aa(global::Quire.EquationEditor.Tree.FormattedString A_0, int A_1, global::System.Nullable<global::Quire.Controls.Core.IdLabels> A_2) => base.aa(A_0, A_1, A_2);
    public bool a(global::Quire.EquationEditor.Tree.FormattedString A_0, int A_1, global::System.Nullable<global::Quire.Controls.Core.IdLabels> A_2, string[] A_3) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool ab(global::Quire.EquationEditor.Tree.MathNode A_0) => base.ab(A_0);
    [global::Quire.Mirror.MirrorStub]
    [MirrorVerified("Release IL")]
    public override bool ac(MathNode mathNode) => ab(mathNode);

    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void ad() => base.ad();
    public override void ae() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void af() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void ag(global::Quire.EquationEditor.Tree.MathNode A_0, global::Quire.Direction A_1) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void ah() => base.ah();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void ai() => base.ai();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void aj() => base.aj();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void ak() => base.ak();
    private void a(int A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void al(global::Quire.EquationEditor.Tree.MathNode A_0, global::Quire.Direction A_1, global::Quire.Direction A_2) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void am() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void an() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void ao() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void a(global::Quire.Delegate1<global::Quire.EquationEditor.Tree.Navigation.NavigatorState> A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override void ap() => base.ap();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    protected override bool aq(global::Quire.Direction A_0, global::Quire.EquationEditor.Tree.MathNode A_1) => base.aq(A_0, A_1);
    protected override global::Quire.Direction ar(global::Quire.EquationEditor.Tree.Navigation.NavigatorState A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool @as(global::Quire.Controls.Core.IdLabels A_0) => base.@as(A_0);
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool at() => base.at();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool au() => base.au();
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    protected override bool av(out global::Quire.EquationEditor.Tree.MathNode A_0) => base.av(out A_0);
    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: base call stub, ordinal placeholder")]
    public override bool aw() => base.aw();
    [global::Quire.Mirror.MirrorOf("l+a, Ptc.EquationEditor")]
    private sealed partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public global::Quire.Native.PtcEquationEditor.l a__;
            [global::Quire.Mirror.MirrorStub]
            public string b;
            internal global::Quire.EquationEditor.Tree.MathNode a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("l+b, Ptc.EquationEditor")]
    private sealed partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public global::Quire.EquationEditor.Tree.MathNode a_;
            internal bool a(global::Quire.EquationEditor.Tree.FunctionOperatorDescriptor A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("l+c, Ptc.EquationEditor")]
    private sealed partial class c
    {
        [MirrorVerified("Release IL")]
        public c() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public global::Quire.Native.PtcEquationEditor.l a_;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.EquationEditor.Tree.LimitSideNodeInfo b;
            internal global::Quire.EquationEditor.Tree.MathNode a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("l+d, Ptc.EquationEditor")]
    private sealed partial class d
    {
        [MirrorVerified("Release IL")]
        public d() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public global::Quire.Native.PtcEquationEditor.l a_;
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("b")]
            public global::Quire.Controls.Core.InputControlTypes b_;
            internal global::Quire.EquationEditor.Tree.MathNode b() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
            internal global::Quire.EquationEditor.Tree.MathNode a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("l+e, Ptc.EquationEditor")]
    private sealed partial class e_
    {
        [MirrorVerified("Release IL")]
        public e_() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public global::Quire.Native.PtcEquationEditor.l a_;
            [global::Quire.Mirror.MirrorStub]
            public string b;
            internal global::Quire.EquationEditor.Tree.MathNode a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("l+f, Ptc.EquationEditor")]
    private sealed partial class f_
    {
        [MirrorVerified("Release IL")]
        public f_() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public string a_;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.Native.PtcEquationEditor.l.e_ b;
            internal global::Quire.EquationEditor.Tree.MathNode a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("l+g, Ptc.EquationEditor")]
    private sealed partial class g_
    {
        [MirrorVerified("Release IL")]
        public g_() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public global::Quire.Native.PtcEquationEditor.l a_;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.EquationEditor.Tree.FormattedString b;
            [global::Quire.Mirror.MirrorStub]
            public global::System.Nullable<global::Quire.Controls.Core.IdLabels> c;
            [global::Quire.Mirror.MirrorStub]
            public int d;
            internal global::Quire.EquationEditor.Tree.MathNode a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("l+h, Ptc.EquationEditor")]
    private sealed partial class h_
    {
        [MirrorVerified("Release IL")]
        public h_() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public global::Quire.EquationEditor.Tree.MathNode[] a_;
            [global::Quire.Mirror.MirrorStub]
            public string[] b;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.Native.PtcEquationEditor.l c;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.EquationEditor.Tree.FormattedString d;
            [global::Quire.Mirror.MirrorStub]
            public global::System.Nullable<global::Quire.Controls.Core.IdLabels> e;
            internal void a(int A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
            internal global::Quire.EquationEditor.Tree.MathNode a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("l+i, Ptc.EquationEditor")]
    private sealed partial class i_
    {
        [MirrorVerified("Release IL")]
        public i_() { }
            [global::Quire.Mirror.MirrorStub]
            [global::Quire.Mirror.MirrorName("a")]
            public global::Quire.EquationEditor.Tree.MathNode a_;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.EquationEditor.Tree.MathNode b;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.EquationEditor.Tree.MathNode c;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.Native.PtcEquationEditor.l d;
            [global::Quire.Mirror.MirrorStub]
            public global::Quire.Direction e;
            internal void a(global::Quire.EquationEditor.Tree.Navigation.NavigatorState A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

}
