#nullable disable
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Scoping;
using Quire.Mirror;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.Equation;

public partial class EquationModel
{
    [MirrorVerified("Release IL")]
    private NavigatorState q;

    [MirrorVerified("Release IL")]
    private void ac() => FireExpressionValueChanged(q, false);

    [MirrorVerified("Release IL")]
    public EquationModel Clone() => new EquationModel(this);

    [MirrorVerified("Release IL")]
    public bool AtLeftEdge() => AtEdge(Direction.Left);

    [MirrorVerified("Release IL")]
    public bool AtRightEdge() => AtEdge(Direction.Right);

    [MirrorVerified("Release IL")]
    public void HandleInputTagChanged(bool isInput) => IsRestrictedReadOnly = isInput;

    [MirrorVerified("Release IL")]
    internal b_ o;

    [MirrorVerified("Release IL")]
    private bool z() => o.a0();

    [MirrorVerified("Release IL")]
    public bool CanCopy() => o.a5();

    [MirrorVerified("Release IL")]
    public bool CanCut() => CanCopy();

    [MirrorVerified("Release IL")]
    public bool CanPaste() => o.d();

    [MirrorVerified("Release IL")]
    public void SelectTo(MathNode node, Direction nodeSide, Direction dragDirection) => o.al(node, nodeSide, dragDirection);

    [MirrorVerified("Release IL")]
    public void Clear() => Clear(false);

    [MirrorVerified("Release IL")]
    public void MoveLeft() => o.z();

    [MirrorVerified("Release IL")]
    public void MoveLeftByTerm() => o.v();

    [MirrorVerified("Release IL")]
    public void MoveRight() => o.y();

    [MirrorVerified("Release IL")]
    public void MoveRightByTerm() => o.u();

    [MirrorVerified("Release IL")]
    public void MoveUp() => o.x();

    [MirrorVerified("Release IL")]
    public void MoveDown() => o.w();

    [MirrorVerified("Release IL")]
    public void JumpToNode(MathNode newNode, Direction caretSide) => o.ag(newNode, caretSide);

    [MirrorVerified("Release IL")]
    public void ExpandScope() => o.ao();

    [MirrorVerified("Release IL")]
    public void CancelScope() => o.a2();

    [MirrorVerified("Release IL")]
    public void ScopeLeft() => o.am();

    [MirrorVerified("Release IL")]
    public void ScopeRight() => o.an();

    [MirrorVerified("Release IL")]
    public void SelectLeft() => o.ah();

    [MirrorVerified("Release IL")]
    public void SelectRight() => o.ai();

    [MirrorVerified("Release IL")]
    public void SelectToEnd() => o.aj();

    [MirrorVerified("Release IL")]
    public void SelectToStart() => o.ak();

    [MirrorVerified("Release IL")]
    public void CancelTypeover() => o.bc();

    [MirrorVerified("Release IL")]
    public void ToggleSubscript() => o.m();

    [MirrorVerified("Release IL")]
    public void MoveToCurrentExpressionStart() => o.r();

    [MirrorVerified("Release IL")]
    public void MoveToCurrentExpressionEnd() => o.q();

    [MirrorVerified("Release IL")]
    public void MoveToOppositeSide() => o.s();

    [MirrorVerified("Release IL")]
    public void SetCurrentOperatorMayBeReplaced(bool currentOperatorMayBeReplaced) => CurrentOperatorMayBeReplaced = currentOperatorMayBeReplaced;

    [MirrorVerified("Release IL")]
    public void MoveToRegionStart() => o.ae();

    [MirrorVerified("Release IL")]
    public void MoveToRegionEnd() => o.af();

    [MirrorVerified("Release IL")]
    public void FullyContractScope() => o.a9();

    [MirrorVerified("Release IL")]
    public void FullyExpandScope() => o.ap();

    [MirrorVerified("Release IL")]
    public void ContractScope() => o.a8();

    [MirrorVerified("Release IL")]
    public bool CanNavigate(Direction direction) => o.p(direction);

    [MirrorVerified("Release IL")]
    public void MoveToCurrentExpressionExtreme(Direction direction) => o.r(direction);

    [MirrorVerified("Release IL")]
    public void FireSynchEvents() => o.t();

    [MirrorVerified("Release IL")]
    internal EquationModel(r A_0, ax A_1) : this(A_0, A_1, null, null, null) { }

    [MirrorVerified("Release IL")]
    private bool i() => o.j();

    [MirrorVerified("Release IL")]
    private bool h() => o.bi();

    [MirrorVerified("Release IL")]
    private bool g() => o.a4();

    [MirrorVerified("Release IL")]
    private bool f() => o.bj();

    [MirrorVerified("Release IL")]
    private bool e() => o.au();

    [MirrorVerified("Release IL")]
    private bool a() => o.at();

    [MirrorVerified("Release IL")]
    private EngineError ax;

    [MirrorVerified("Release IL")]
    public EngineError Error => ax;

    [MirrorVerified("Release IL")]
    private IList<EngineRedefInfo> ay;

    [MirrorVerified("Release IL")]
    public IList<EngineRedefInfo> EngineWarnings { get => ay; set => ay = value; }

    [MirrorVerified("Release IL")]
    public EquationEditModeEnum Mode => o.e();

    [MirrorVerified("Release IL")]
    private bool az;

    [MirrorVerified("Release IL")]
    public bool IsReplacingPreviouseOperator { get => az; set => az = value; }

    [MirrorVerified("Release IL")]
    private bool a0;

    [MirrorVerified("Release IL")]
    public bool CurrentOperatorMayBeReplaced { get => a0; set => a0 = value; }

    [MirrorVerified("Release IL")]
    private Direction a1;

    [MirrorVerified("Release IL")]
    public Direction ReplaceDirectionForMultiKeyOp { get => a1; set => a1 = value; }

    [MirrorVerified("Release IL")]
    private bool a2;

    [MirrorVerified("Release IL")]
    public bool PrevStepForMultiKeyOpFailed { get => a2; set => a2 = value; }

    [MirrorVerified("Release IL")]
    private IResultNode a4;

    [MirrorVerified("Release IL")]
    public IResultNode EngineResultForSerialization { get => a4; set => a4 = value; }

    [MirrorVerified("Release IL")]
    private MathNode x;

    [MirrorVerified("Release IL")]
    public MathNode ErrorNode { get => x; set => x = value; }

    [MirrorVerified("Release IL")]
    internal TypeoverState p;

    [MirrorVerified("Release IL")]
    public TypeoverState CurrentTypeoverState => p;

    [MirrorVerified("Release IL")]
    private bool a7;

    [MirrorVerified("Release IL")]
    public bool IsModified { get => a7; set => a7 = value; }

    [MirrorVerified("Release IL")]
    public string ScopedSExpression => o.ay();

    [MirrorVerified("Release IL")]
    public Direction ScopeSide => o.ax();

    [MirrorVerified("Release IL")]
    private bool ba;

    [MirrorVerified("Release IL")]
    public bool IsMathFormatHandledLocally { get => ba; set => ba = value; }

    [MirrorVerified("Release IL")]
    private bool bc;

    [MirrorVerified("Release IL")]
    public bool OverrideIdLabelByResolvedForDefinitionLhs { get => bc; set => bc = value; }

    [MirrorVerified("Release IL")]
    public string TypeoverSExpression => o.ba();

    [MirrorVerified("Release IL")]
    public ScopedNodes ScopedNodesByLines => o.az();

    [MirrorVerified("Release IL")]
    public IList<MathNode> TypeoverNodes => o.bb();

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public virtual Direction ax() => Direction.Indeterminate;

        [MirrorVerified("Release IL")]
        public virtual string ay() => null;

        [MirrorVerified("Release IL")]
        public virtual string ba() => null;

        [MirrorVerified("Release IL")]
        public void z() => k(Direction.Left);

        [MirrorVerified("Release IL")]
        public void y() => k(Direction.Right);

        [MirrorVerified("Release IL")]
        public void x() => k(Direction.Up);

        [MirrorVerified("Release IL")]
        public void w() => k(Direction.Down);

        [MirrorVerified("Release IL")]
        public void v() => q(Direction.Left);

        [MirrorVerified("Release IL")]
        public void u() => q(Direction.Right);

        [MirrorVerified("Release IL")]
        public void r() => r(Direction.Left);

        [MirrorVerified("Release IL")]
        public void q() => r(Direction.Right);

        [MirrorVerified("Release IL")]
        public virtual void a8() { }

        [MirrorVerified("Release IL")]
        public virtual void ap() { }

        [MirrorVerified("Release IL")]
        public virtual void a9() { }

        [MirrorVerified("Release IL")]
        public virtual void a2() => throw new InvalidOperationException("You need to call BeginScope before actually scoping in a direction");

        [MirrorVerified("Release IL")]
        public virtual void ah() { }

        [MirrorVerified("Release IL")]
        public virtual void ai() { }

        [MirrorVerified("Release IL")]
        public virtual void ak() { }

        [MirrorVerified("Release IL")]
        public virtual void aj() { }

        [MirrorVerified("Release IL")]
        public virtual void bc() => throw new InvalidOperationException("not in typeover mode");

        [MirrorVerified("Release IL")]
        public virtual bool x(FormattedString A_0) => false;

        [MirrorVerified("Release IL")]
        public virtual bool z(string A_0) => false;

        [MirrorVerified("Release IL")]
        public virtual bool y(InputControlTypes A_0) => false;

        [MirrorVerified("Release IL")]
        public virtual bool a4(FindReplaceQuery A_0) => false;

        [MirrorVerified("Release IL")]
        public virtual bool aa(FormattedString A_0, int A_1, Nullable<IdLabels> A_2) => false;

        [MirrorVerified("Release IL")]
        public virtual bool au() => false;

        [MirrorVerified("Release IL")]
        public virtual bool n() => false;

        [MirrorVerified("Release IL")]
        public virtual bool a5() => false;

        [MirrorVerified("Release IL")]
        public virtual void a6() { }

        [MirrorVerified("Release IL")]
        public virtual bool a7() => false;

        [MirrorVerified("Release IL")]
        public virtual bool aw() => false;

        [MirrorVerified("Release IL")]
        public virtual bool l() => false;

        [MirrorVerified("Release IL")]
        public virtual bool o() => false;

        [MirrorVerified("Release IL")]
        public virtual void m() { }

        [MirrorVerified("Release IL")]
        protected virtual bool aq(Direction A_0, MathNode A_1) => false;

        [MirrorVerified("Release IL")]
        public virtual bool p(Direction A_0) => true;

        [MirrorVerified("Release IL")]
        public virtual bool ab(MathNode A_0) => false;

        [MirrorVerified("Release IL")]
        public virtual bool ac(MathNode A_0) => false;

        [MirrorVerified("Release IL")]
        public virtual bool a3(FindReplaceQuery A_0) => false;
    }

    partial class e_
    {
        [MirrorVerified("Release IL")]
        private bool h;

        [MirrorVerified("Release IL")]
        public bool b() => h;

        [MirrorVerified("Release IL")]
        public void a(bool A_0) => h = A_0;
    }

    partial class f_
    {
        [MirrorVerified("Release IL")]
        public f_() { }

        [MirrorVerified("Release IL")]
        internal bool a(MathNode A_0) => A_0.IsReadonly;
    }

    partial class g_
    {
        [MirrorVerified("Release IL")]
        public g_() { }
    }

    partial class h_
    {
        [MirrorVerified("Release IL")]
        public h_() { }
    }

    partial class i_
    {
        [MirrorVerified("Release IL")]
        public i_() { }
    }

    partial class j_
    {
        [MirrorVerified("Release IL")]
        public j_() { }
    }

    partial class k_
    {
        [MirrorVerified("Release IL")]
        public k_() { }
    }

    partial class l_
    {
        [MirrorVerified("Release IL")]
        public l_() { }
    }

    partial class m_
    {
        [MirrorVerified("Release IL")]
        public m_() { }
    }

    partial class n_
    {
        [MirrorVerified("Release IL")]
        public n_() { }
    }

    partial class o_
    {
        [MirrorVerified("Release IL")]
        public o_() { }
    }

    partial class p_
    {
        [MirrorVerified("Release IL")]
        public p_() { }
    }

    partial class q_
    {
        [MirrorVerified("Release IL")]
        public q_() { }
    }

    partial class r_
    {
        [MirrorVerified("Release IL")]
        public r_() { }
    }

    partial class s_
    {
        [MirrorVerified("Release IL")]
        public s_() { }
    }

    partial class t_
    {
        [MirrorVerified("Release IL")]
        public t_() { }
    }

    partial class u_
    {
        [MirrorVerified("Release IL")]
        public u_() { }
    }

    partial class v_
    {
        [MirrorVerified("Release IL")]
        public v_() { }
    }

    partial class w_
    {
        [MirrorVerified("Release IL")]
        public w_() { }
    }

    partial class x_
    {
        [MirrorVerified("Release IL")]
        public x_() { }
    }

    partial class y_
    {
        [MirrorVerified("Release IL")]
        public y_() { }
    }

    partial class z_
    {
        [MirrorVerified("Release IL")]
        public z_() { }
    }

    partial class aa_
    {
        [MirrorVerified("Release IL")]
        public aa_() { }
    }

    partial class ab_
    {
        [MirrorVerified("Release IL")]
        public ab_() { }
    }

    partial class ac_
    {
        [MirrorVerified("Release IL")]
        public ac_() { }
    }

    partial class ad_
    {
        [MirrorVerified("Release IL")]
        public ad_() { }
    }

    partial class ae_
    {
        [MirrorVerified("Release IL")]
        public ae_() { }
    }

    partial class af_
    {
        [MirrorVerified("Release IL")]
        public af_() { }
    }

    partial class ag_
    {
        [MirrorVerified("Release IL")]
        public ag_() { }
    }

    partial class ah_
    {
        [MirrorVerified("Release IL")]
        public ah_() { }
    }

    partial class ai_
    {
        [MirrorVerified("Release IL")]
        public ai_() { }
    }

    partial class a_
    {
        partial class b_
        {
            [MirrorVerified("Release IL")]
            public b_() { }
        }
    }

    partial class a_
    {
        partial class c
        {
            [MirrorVerified("Release IL")]
            public c() { }
        }
    }

    partial class a_
    {
        partial class d
        {
            [MirrorVerified("Release IL")]
            public d() { }
        }
    }

    partial class a_
    {
        partial class e
        {
            [MirrorVerified("Release IL")]
            public e() { }
        }
    }

    partial class c_
    {
        partial class a_
        {
            [MirrorVerified("Release IL")]
            public a_() { }
        }
    }
}
