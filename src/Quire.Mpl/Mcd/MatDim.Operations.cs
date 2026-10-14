#nullable disable
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

public abstract partial class MatDim
{

    [MirrorAnalyzed("Mcd.MatDim")]
    public static bool Eq_b(MatDim d1, MatDim d2) => d1.eq_b(d2);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static bool Eq_b(MatDim d1, Dim d2) => d1.eq2_b(d2);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static void Eq(MatDim d1, MatDim d2, uint param)
    {
        if (!Eq_b(d1, d2)) PickLoser(d1, d2, param);
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    public static void Eq(MatDim d1, Dim d2, uint param)
    {
        if (!Eq_b(d1, d2)) OnError(d2, param);
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim MMult(MatDim m1, MatDim m2) => m1.mmult1(m2);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static Dim DMult(MatDim m1, MatDim m2) => m1.dmult1(m2);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim Transpose(MatDim m1) => m1.transpose();
    [MirrorAnalyzed("Mcd.MatDim")]
    public static Dim Det(MatDim m, int n) => m.det(n);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim Pwr(MatDim m, int n)
    {
        if (n > 0) return m.pwr((uint)n);
        if (n < 0)
        {
            var inverse = m.invert();
            return n == -1 || inverse == null ? inverse : inverse.pwr(unchecked((uint)-n));
        }
        return m.pwr(2) == null ? null : MMult(m, m.invert());
    }

    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual MatDim mmult1(MatDim m2) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual MatDim mmult2(uniform_dim m1) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual MatDim mmult2(r_dim m1) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual MatDim mmult2(c_dim m1) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual MatDim mmult2(rc_dim m1) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual Dim dmult1(MatDim m2) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual Dim dmult2(uniform_dim m1) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual Dim dmult2(r_dim m1) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual Dim dmult2(c_dim m1) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual Dim dmult2(rc_dim m1) => null;
    protected abstract bool eq_b(MatDim m2);
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual bool eq2_b(Dim d2) => false;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual bool eq2_b(r_dim m1) => false;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual bool eq2_b(c_dim m1) => false;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual bool eq2_b(rc_dim m1) => false;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual bool eq2_b(crazy_dim m1) => false;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual MatDim transpose() => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual MatDim invert() => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual Dim det(int n) => null;
    [MirrorAnalyzed("Mcd.MatDim")]
    protected virtual MatDim pwr(uint n) => null;

    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim make(Dim d) => uniform_dim.make(d, 1, 1);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim make(MatVal m) => make(m, false);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim make(MatVal m, bool linear_algebra_dimensions_only)
    {
        if (m.IsUniform) return uniform_dim.make(m.m_dim, m.Rows, m.Cols);
        if (m.Cols == 1) return makeColumnVectorDim(m);
        if (m.Rows == 1) return makeRowVectorDim(m);
        return makeArrayDim(m, linear_algebra_dimensions_only);
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim make(RC rc, Dim[] dims, int n)
    {
        Dim common = Uniform(dims);
        if (common != null) return uniform_dim.make(common, dims.Length, dims.Length);
        return rc == RC.ROW ? r_dim.make(dims, n) : c_dim.make(dims, n);
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim makeRowDimension(Dim d1, Dim d2, int c) => make(RC.ROW, [d1, d2], c);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim makeRowDimension(Dim d1, Dim[] d2, int c) => make(RC.ROW, Push(d1, d2), c);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim makeColDimension(Dim d1, Dim d2, int r) => make(RC.COL, [d1, d2], r);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim makeColDimension(Dim d1, Dim[] d2, int r) => make(RC.COL, Push(d1, d2), r);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim make(Dim[] rows, Dim[] cols)
    {
        Dim common = Uniform(rows);
        if (common != null) return make(RC.COL, Mult(common, cols), rows.Length);
        common = Uniform(cols);
        return common != null ? make(RC.ROW, Mult(common, rows), cols.Length) : rc_dim.make_rc(rows, cols);
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    private static MatDim makeColumnVectorDim(MatVal m)
    {
        var dims = m.ColDimArray(0);
        return dims == null ? crazy_dim.make(m) : r_dim.make(dims, m.Cols);
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    private static MatDim makeRowVectorDim(MatVal m)
    {
        var dims = m.RowDimArray(0);
        return dims == null ? crazy_dim.make(m) : c_dim.make(dims, m.Rows);
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    private static MatDim makeArrayDim(MatVal m, bool linear_algebra_dimensions_only)
    {
        var result = makeArrayDim(m);
        return linear_algebra_dimensions_only ? result.IsLinear : result;
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    private static MatDim makeArrayDim(MatVal m)
    {
        var rows = m.ColDimArray(0);
        var cols = m.RowDimArray(0);
        if (Uniform(rows) != null)
        {
            for (int c = 1; c < m.Cols; c++) if (Uniform(m.ColDimArray(c)) == null) return crazy_dim.make(m);
            return c_dim.make(cols, rows.Length);
        }
        if (Uniform(cols) != null)
        {
            for (int r = 1; r < m.Rows; r++) if (Uniform(m.RowDimArray(r)) == null) return crazy_dim.make(m);
            return r_dim.make(rows, cols.Length);
        }
        if (rows == null || cols == null) return crazy_dim.make(m);
        var factors = new Dim[m.Cols];
        factors[0] = Dim.identity;
        for (int c = 1; c < factors.Length; c++)
        {
            factors[c] = IsScalarMultiple(rows, m.ColDimArray(c));
            if (factors[c] == null) return crazy_dim.make(m);
        }
        return rc_dim.make_rc(rows, factors);
    }

    [MirrorAnalyzed("Mcd.MatDim")]
    public static void OnError(string m, Dim d, uint a) => AppException.Raise($"{m}\t{Dim.FixDimension(d).Format()}", a);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static void OnError(Dim d, uint a) => OnError("bad matrix dimension", d, a);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static void OnError(string m, Dim d, int i, uint a) => AppException.Raise($"{m}\t{Dim.FixDimension(d).Format()}\t{i}", a);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static void OnError(string m, Dim d, int i, int j, uint a) => AppException.Raise($"{m}\t{Dim.FixDimension(d).Format()}\t{i}\t{j}", a);
    [MirrorAnalyzed("Mcd.MatDim")]
    private static void PickLoser(MatDim loser, MatDim winner, uint param)
    {
        for (int r = 0; r < loser.Rows; r++)
        for (int c = 0; c < loser.Cols; c++)
        {
            if (Dim.Eq_b(loser.dim(r, c), winner.dim(r, c))) continue;
            if (loser.Cols == 1) OnError("bad vector element dimension", winner.dim(r, c), r, param);
            else OnError("bad matrix element dimension", winner.dim(r, c), r, c, param);
        }
        AppException.Raise("array unit mismatch", param);
    }

    protected partial class uniform_dim : MatDim
    {
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim m_dim;
        [MirrorAnalyzed("Mcd.MatDim")]
        private uniform_dim(Dim d, int r, int c) : base(r, c) => m_dim = d ?? Dim.identity;
        [MirrorAnalyzed("Mcd.MatDim")]
        public static uniform_dim make(Dim d, int r, int c) => new(d, r, c);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim UniformDim => m_dim;
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim IsLinear => this;
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim dim(int i, int j) => m_dim;
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim matdim(int i, int j) => this;
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(m_dim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq2_b(Dim d2) => Dim.Eq_b(m_dim, d2);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult1(MatDim m2) => m2.mmult2(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult2(uniform_dim m1) => make(Dim.Mult(m1.m_dim, m_dim), m1.Rows, Cols);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult2(r_dim m1) => r_dim.make(Mult(m_dim, m1.m_rowdim), Cols);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim dmult1(MatDim m2) => m2.dmult2(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim dmult2(uniform_dim m1) => Dim.Mult(m1.m_dim, m_dim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim transpose() => this;
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim invert() => make(Dim.Pwr(m_dim, -1), Rows, Cols);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim det(int n) => Dim.Pwr(m_dim, n);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim pwr(uint n) => make(Dim.Pwr(m_dim, n), Rows, Cols);
    }
    protected partial class r_dim : MatDim
    {
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim[] m_rowdim;
        [MirrorAnalyzed("Mcd.MatDim")]
        private r_dim(Dim[] r, int c) : base(r.Length, c) => m_rowdim = r;
        [MirrorAnalyzed("Mcd.MatDim")]
        public static r_dim make(Dim[] r, int n) => new(r, n);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim IsLinear => this;
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim[] RowDimensions => m_rowdim;
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim dim(int i, int j) => m_rowdim[i];
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim matdim(int i, int j) => uniform_dim.make(m_rowdim[i], 1, 1);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim[] ColDimensionArray(int c) => m_rowdim;
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq2_b(r_dim m1) => Eq_b(m_rowdim, m1.m_rowdim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult1(MatDim m2) => m2.mmult2(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult2(c_dim m1)
        {
            Dim factor = Mult(m1.m_coldim, m_rowdim);
            return factor == null ? null : uniform_dim.make(factor, m1.Rows, Cols);
        }
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult2(rc_dim m1)
        {
            Dim factor = Mult(m1.m_coldim, m_rowdim);
            return factor == null ? null : make(Mult(factor, m1.m_rowdim), Cols);
        }
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim dmult1(MatDim m2) => m2.dmult2(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim dmult2(r_dim m1) => Mult(m1.m_rowdim, m_rowdim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim dmult2(c_dim m1) => Mult(m1.m_coldim, m_rowdim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim transpose() => c_dim.make(m_rowdim, Cols);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim invert() => c_dim.make(Invert(m_rowdim), Rows);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim det(int n) => Product(Dim.identity, m_rowdim);
    }
    protected partial class c_dim : MatDim
    {
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim[] m_coldim;
        [MirrorAnalyzed("Mcd.MatDim")]
        private c_dim(Dim[] c, int r) : base(r, c.Length) => m_coldim = c;
        [MirrorAnalyzed("Mcd.MatDim")]
        public static c_dim make(Dim[] c, int r) => new(c, r);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim IsLinear => this;
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim[] ColDimensions => m_coldim;
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim dim(int i, int j) => m_coldim[j];
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim matdim(int i, int j) => uniform_dim.make(m_coldim[j], 1, 1);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim[] RowDimensionArray(int r) => m_coldim;
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq2_b(c_dim m1) => Eq_b(m_coldim, m1.m_coldim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult1(MatDim m2) => m2.mmult2(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult2(uniform_dim m1) => make(Mult(m1.m_dim, m_coldim), m1.Rows);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult2(r_dim m1) => rc_dim.make_rc(m1.m_rowdim, m_coldim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim dmult1(MatDim m2) => m2.dmult2(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim dmult2(r_dim m1) => Mult(m1.m_rowdim, m_coldim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim dmult2(c_dim m1) => Mult(m1.m_coldim, m_coldim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim transpose() => r_dim.make(m_coldim, Rows);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim invert() => r_dim.make(Invert(m_coldim), m_coldim.Length);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim det(int n) => Product(Dim.identity, m_coldim);
    }
    protected partial class rc_dim : MatDim
    {
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim[] m_rowdim;
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim[] m_coldim;
        [MirrorAnalyzed("Mcd.MatDim")]
        private rc_dim(Dim[] r, Dim[] c) : base(r.Length, c.Length) { m_rowdim = r; m_coldim = c; }
        [MirrorAnalyzed("Mcd.MatDim")]
        public static rc_dim make_rc(Dim[] r, Dim[] c) => new(r, c);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim IsLinear => this;
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim dim(int i, int j) => Dim.Mult(m_rowdim[i], m_coldim[j]);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim matdim(int i, int j) => uniform_dim.make(dim(i, j), 1, 1);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim[] RowDimensionArray(int r) => Mult(m_rowdim[r], m_coldim);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim[] ColDimensionArray(int c) => Mult(m_coldim[c], m_rowdim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq2_b(rc_dim m1) => Eq_b(m_rowdim, m1.m_rowdim) & Eq_b(m_coldim, m1.m_coldim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult1(MatDim m2) => m2.mmult2(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult2(c_dim m1)
        {
            Dim factor = Mult(m1.m_coldim, m_rowdim);
            return factor == null ? null : c_dim.make(Mult(factor, m_coldim), m1.Rows);
        }
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim mmult2(rc_dim m1)
        {
            Dim factor = Mult(m1.m_coldim, m_rowdim);
            return factor == null ? null : make_rc(Mult(factor, m1.m_rowdim), m_coldim);
        }
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim transpose() => make_rc(m_coldim, m_rowdim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim invert() => make_rc(Invert(m_coldim), Invert(m_rowdim));
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override Dim det(int n) => Product(Product(Dim.identity, m_rowdim), m_coldim);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override MatDim pwr(uint n)
        {
            Dim factor = Mult(m_coldim, m_rowdim);
            return factor == null ? null : make_rc(Mult(Dim.Pwr(factor, unchecked(n - 1)), m_rowdim), m_coldim);
        }
    }
    protected partial class crazy_dim : MatDim
    {
        [MirrorAnalyzed("Mcd.MatDim")]
        private readonly MatVal m_matrix;
        [MirrorAnalyzed("Mcd.MatDim")]
        protected crazy_dim(MatVal m) : base(m.Rows, m.Cols) => m_matrix = m;
        [MirrorAnalyzed("Mcd.MatDim")]
        public static new crazy_dim make(MatVal m) => new(m);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override Dim dim(int i, int j) => m_matrix.get_2_dim(i, j);
        [MirrorAnalyzed("Mcd.MatDim")]
        public override MatDim matdim(int i, int j) => m_matrix.get_2_matdim(i, j);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(this);
        [MirrorAnalyzed("Mcd.MatDim")]
        protected override bool eq2_b(crazy_dim m1)
        {
            if ((Rows != m1.Rows) | (Cols != m1.Cols)) return false;
            for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (!Eq_b(matdim(r, c), m1.matdim(r, c))) return false;
            return true;
        }
    }
}
