#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public partial class MathNodeToRlTransformer
{
    [MirrorVerified("Release IL")]
    private string z(MathNode mathNode) => b(mathNode, "setf");

    [MirrorVerified("Release IL")]
    private string y(MathNode mathNode) => b(mathNode, "global");

    partial class PositionsToLabel
    {
        [MirrorVerified("Release IL")]
        public PositionsToLabel(int[] positions, string label) : base(positions, label) { }

        [MirrorVerified("Release IL")]
        public int[] Positions => base.Item1;

        [MirrorVerified("Release IL")]
        public string Label => base.Item2;
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

    partial class d_
    {
        [MirrorVerified("Release IL")]
        public d_() { }
    }

    partial class e_
    {
        [MirrorVerified("Release IL")]
        public e_() { }
    }

    partial class f_
    {
        [MirrorVerified("Release IL")]
        public f_() { }
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

    partial class a_
    {
        partial class a__
        {
            [MirrorVerified("Release IL")]
            public a__() { }
        }
    }

    partial class a_
    {
        partial class b_
        {
            [MirrorVerified("Release IL")]
            public b_() { }
        }
    }
}
