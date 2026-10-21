#nullable disable
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

public abstract partial class MatDim
{

    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public static bool Eq_b(MatDim d1, MatDim d2) => d1.eq_b(d2);
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public static bool Eq_b(MatDim d1, Dim d2) => d1.eq2_b(d2);
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public static void Eq(MatDim d1, MatDim d2, uint param)
    {
        if (!d1.eq_b(d2)) PickLoser(d1, d2, param);
    }
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public static void Eq(MatDim d1, Dim d2, uint param)
    {
        if (!d1.eq2_b(d2)) OnError(d2, param);
    }
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public static MatDim MMult(MatDim m1, MatDim m2) => m1.mmult1(m2);
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public static Dim DMult(MatDim m1, MatDim m2) => m1.dmult1(m2);
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public static MatDim Transpose(MatDim m1) => m1.transpose();
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    public static Dim Det(MatDim m, int n) => m.det(n);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim Pwr(MatDim m, int n)
    {
        if (n > 0)
        {
            return m.pwr((uint)n);
        }
        if (n < 0)
        {
            MatDim matDim = m.invert();
            if (n != -1 && matDim != null)
            {
                return matDim.pwr((uint)(-n));
            }
            return matDim;
        }
        if (m.pwr(2u) == null)
        {
            return null;
        }
        return MMult(m, m.invert());
    }

    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual MatDim mmult1(MatDim m2) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual MatDim mmult2(uniform_dim m1) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual MatDim mmult2(r_dim m1) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual MatDim mmult2(c_dim m1) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual MatDim mmult2(rc_dim m1) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual Dim dmult1(MatDim m2) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual Dim dmult2(uniform_dim m1) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual Dim dmult2(r_dim m1) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual Dim dmult2(c_dim m1) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual Dim dmult2(rc_dim m1) => null;
    protected abstract bool eq_b(MatDim m2);
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual bool eq2_b(Dim d2) => false;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual bool eq2_b(r_dim m1) => false;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual bool eq2_b(c_dim m1) => false;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual bool eq2_b(rc_dim m1) => false;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual bool eq2_b(crazy_dim m1) => false;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual MatDim transpose() => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual MatDim invert() => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
    protected virtual Dim det(int n) => null;
    [MirrorVerified("MatDimOperationsDifferentialTests")]
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
        if (common == null) return rc == RC.ROW ? r_dim.make(dims, n) : c_dim.make(dims, n);
        return uniform_dim.make(common, dims.Length, dims.Length);
    }
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim makeRowDimension(Dim d1, Dim d2, int c) => make(RC.ROW, new Dim[2] { d1, d2 }, c);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim makeRowDimension(Dim d1, Dim[] d2, int c) => make(RC.ROW, Push(d1, d2), c);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim makeColDimension(Dim d1, Dim d2, int r) => make(RC.COL, new Dim[2] { d1, d2 }, r);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim makeColDimension(Dim d1, Dim[] d2, int r) => make(RC.COL, Push(d1, d2), r);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim make(Dim[] rows, Dim[] cols)
    {
        Dim dim = Uniform(rows);
        if (dim != null)
        {
            return make(RC.COL, Mult(dim, cols), rows.Length);
        }
        Dim dim2 = Uniform(cols);
        if (dim2 != null)
        {
            return make(RC.ROW, Mult(dim2, rows), cols.Length);
        }
        return rc_dim.make_rc(rows, cols);
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
        Dim[] array = m.ColDimArray(0);
        Dim[] array2 = m.RowDimArray(0);
        if (Uniform(array) != null)
        {
            for (int i = 1; i < m.Cols; i++)
            {
                if (Uniform(m.ColDimArray(i)) == null)
                {
                    return crazy_dim.make(m);
                }
            }
            return c_dim.make(array2, array.Length);
        }
        if (Uniform(array2) != null)
        {
            for (int j = 1; j < m.Rows; j++)
            {
                if (Uniform(m.RowDimArray(j)) == null)
                {
                    return crazy_dim.make(m);
                }
            }
            return r_dim.make(array, array2.Length);
        }
        if (array == null || array2 == null)
        {
            return crazy_dim.make(m);
        }
        Dim[] array3 = new Dim[m.Cols];
        array3[0] = Dim.identity;
        for (int k = 1; k < m.Cols; k++)
        {
            Dim dim = IsScalarMultiple(array, m.ColDimArray(k));
            if (dim == null)
            {
                return crazy_dim.make(m);
            }
            array3[k] = dim;
        }
        return rc_dim.make_rc(array, array3);
    }

    [MirrorAnalyzed("Mcd.MatDim")]
    public static void OnError(string m, Dim d, uint a) => AppException.Raise(string.Format("{0}\t{1}", m, Dim.FixDimension(d).Format()), a);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static void OnError(Dim d, uint a) => OnError("bad matrix dimension", d, a);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static void OnError(string m, Dim d, int i, uint a) => AppException.Raise(string.Format("{0}\t{1}\t{2}", m, Dim.FixDimension(d).Format(), i), a);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static void OnError(string m, Dim d, int i, int j, uint a) => AppException.Raise(string.Format("{0}\t{1}\t{2}\t{3}", m, Dim.FixDimension(d).Format(), i, j), a);
    [MirrorAnalyzed("Mcd.MatDim")]
    private static void PickLoser(MatDim loser, MatDim winner, uint param)
    {
        int rows = loser.Rows;
        int cols = loser.Cols;
        if (cols == 1)
        {
            for (int i = 0; i < rows; i++)
            {
                if (!Dim.Eq_b(loser.dim(i, 0), winner.dim(i, 0)))
                {
                    OnError("bad vector element dimension", winner.dim(i, 0), i, param);
                }
            }
            AppException.Raise("array unit mismatch", param);
            return;
        }
        for (int j = 0; j < rows; j++)
        {
            for (int k = 0; k < cols; k++)
            {
                if (!Dim.Eq_b(loser.dim(j, k), winner.dim(j, k)))
                {
                    OnError("bad matrix element dimension", winner.dim(j, k), j, k, param);
                }
            }
        }
        AppException.Raise("array unit mismatch", param);
    }

    protected partial class uniform_dim : MatDim
    {
        [MirrorVerified("MatDimUniformDimConstructionDifferentialTests")]
        public readonly Dim m_dim;
        [MirrorVerified("MatDimUniformDimConstructionDifferentialTests")]
        private uniform_dim(Dim d, int r, int c) : base(r, c) => m_dim = d != null ? d : Dim.identity;
        [MirrorVerified("MatDimUniformDimConstructionDifferentialTests")]
        public static uniform_dim make(Dim d, int r, int c) => new(d, r, c);
        [MirrorVerified("MatDimUniformDimConstructionDifferentialTests")]
        public override Dim UniformDim => m_dim;
        [MirrorVerified("MatDimUniformDimConstructionDifferentialTests")]
        public override MatDim IsLinear => this;
        [MirrorVerified("MatDimUniformDimConstructionDifferentialTests")]
        public override Dim dim(int i, int j) => m_dim;
        [MirrorVerified("MatDimUniformDimConstructionDifferentialTests")]
        public override MatDim matdim(int i, int j) => this;
        [MirrorVerified("MatDimUniformEqBDifferentialTests")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(m_dim);
        [MirrorVerified("MatDimUniformEq2BDifferentialTests")]
        protected override bool eq2_b(Dim d2) => Dim.Eq_b(m_dim, d2);
        [MirrorVerified("MatDimUniformDimArithmeticDifferentialTests")]
        protected override MatDim mmult1(MatDim m2) => m2.mmult2(this);
        [MirrorVerified("MatDimUniformDimArithmeticDifferentialTests")]
        protected override MatDim mmult2(uniform_dim m1) => make(Dim.Mult(m1.m_dim, m_dim), m1.Rows, Cols);
        [MirrorVerified("MatDimUniformDimArithmeticDifferentialTests")]
        protected override MatDim mmult2(r_dim m1) => r_dim.make(Mult(m_dim, m1.m_rowdim), Cols);
        [MirrorVerified("MatDimUniformDimArithmeticDifferentialTests")]
        protected override Dim dmult1(MatDim m2) => m2.dmult2(this);
        [MirrorVerified("MatDimUniformDimArithmeticDifferentialTests")]
        protected override Dim dmult2(uniform_dim m1) => Dim.Mult(m1.m_dim, m_dim);
        [MirrorVerified("MatDimUniformDimConstructionDifferentialTests")]
        protected override MatDim transpose() => this;
        [MirrorVerified("MatDimUniformDimArithmeticDifferentialTests")]
        protected override MatDim invert() => make(Dim.Pwr(m_dim, -1), Rows, Cols);
        [MirrorVerified("MatDimUniformDimArithmeticDifferentialTests")]
        protected override Dim det(int n) => Dim.Pwr(m_dim, n);
        [MirrorVerified("MatDimUniformDimArithmeticDifferentialTests")]
        protected override MatDim pwr(uint n) => make(Dim.Pwr(m_dim, n), Rows, Cols);
    }
    protected partial class r_dim : MatDim
    {
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim[] m_rowdim;
        [MirrorVerified("MatDimRDimConstructorDifferentialTests")]
        private r_dim(Dim[] r, int c) : base(r.Length, c) => m_rowdim = r;
        [MirrorVerified("MatDimRDimConstructorDifferentialTests")]
        public static r_dim make(Dim[] r, int n) => new(r, n);
        [MirrorVerified("MatDimRDimCoordinatesDifferentialTests")]
        public override MatDim IsLinear => this;
        [MirrorVerified("MatDimRDimCoordinatesDifferentialTests")]
        public override Dim[] RowDimensions => m_rowdim;
        [MirrorVerified("MatDimRDimCoordinatesDifferentialTests")]
        public override Dim dim(int i, int j) => m_rowdim[i];
        [MirrorVerified("MatDimRDimCoordinatesDifferentialTests")]
        public override MatDim matdim(int i, int j) => uniform_dim.make(m_rowdim[i], 1, 1);
        [MirrorVerified("MatDimRDimCoordinatesDifferentialTests")]
        public override Dim[] ColDimensionArray(int c) => m_rowdim;
        [MirrorVerified("MatDimRDimEqualityDifferentialTests")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(this);
        [MirrorVerified("MatDimRDimEqualityDifferentialTests")]
        protected override bool eq2_b(r_dim m1) => Eq_b(m_rowdim, m1.m_rowdim);
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override MatDim mmult1(MatDim m2) => m2.mmult2(this);
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override MatDim mmult2(c_dim m1)
        {
            Dim factor = Mult(m1.m_coldim, m_rowdim);
            return factor == null ? null : uniform_dim.make(factor, m1.Rows, Cols);
        }
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override MatDim mmult2(rc_dim m1)
        {
            Dim factor = Mult(m1.m_coldim, m_rowdim);
            return factor == null ? null : make(Mult(factor, m1.m_rowdim), Cols);
        }
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override Dim dmult1(MatDim m2) => m2.dmult2(this);
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override Dim dmult2(r_dim m1) => Mult(m1.m_rowdim, m_rowdim);
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override Dim dmult2(c_dim m1) => Mult(m1.m_coldim, m_rowdim);
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override MatDim transpose() => c_dim.make(m_rowdim, Cols);
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override MatDim invert() => c_dim.make(Invert(m_rowdim), Rows);
        [MirrorVerified("MatDimRDimArithmeticDifferentialTests")]
        protected override Dim det(int n) => Product(Dim.identity, m_rowdim);
    }
    protected partial class c_dim : MatDim
    {
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim[] m_coldim;
        [MirrorVerified("MatDimCDimConstructorDifferentialTests")]
        private c_dim(Dim[] c, int r) : base(r, c.Length) => m_coldim = c;
        [MirrorVerified("MatDimCDimConstructorDifferentialTests")]
        public static c_dim make(Dim[] c, int r) => new(c, r);
        [MirrorVerified("MatDimCDimCoordinatesDifferentialTests")]
        public override MatDim IsLinear => this;
        [MirrorVerified("MatDimCDimCoordinatesDifferentialTests")]
        public override Dim[] ColDimensions => m_coldim;
        [MirrorVerified("MatDimCDimCoordinatesDifferentialTests")]
        public override Dim dim(int i, int j) => m_coldim[j];
        [MirrorVerified("MatDimCDimCoordinatesDifferentialTests")]
        public override MatDim matdim(int i, int j) => uniform_dim.make(m_coldim[j], 1, 1);
        [MirrorVerified("MatDimCDimCoordinatesDifferentialTests")]
        public override Dim[] RowDimensionArray(int r) => m_coldim;
        [MirrorVerified("MatDimCDimEqualityDifferentialTests")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(this);
        [MirrorVerified("MatDimCDimEqualityDifferentialTests")]
        protected override bool eq2_b(c_dim m1) => Eq_b(m_coldim, m1.m_coldim);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override MatDim mmult1(MatDim m2) => m2.mmult2(this);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override MatDim mmult2(uniform_dim m1) => make(Mult(m1.m_dim, m_coldim), m1.Rows);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override MatDim mmult2(r_dim m1) => rc_dim.make_rc(m1.m_rowdim, m_coldim);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override Dim dmult1(MatDim m2) => m2.dmult2(this);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override Dim dmult2(r_dim m1) => Mult(m1.m_rowdim, m_coldim);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override Dim dmult2(c_dim m1) => Mult(m1.m_coldim, m_coldim);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override MatDim transpose() => r_dim.make(m_coldim, Rows);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override Dim det(int n) => Product(Dim.identity, m_coldim);
        [MirrorVerified("MatDimCDimArithmeticDifferentialTests")]
        protected override MatDim invert() => r_dim.make(Invert(m_coldim), m_coldim.Length);
    }
    protected partial class rc_dim : MatDim
    {
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim[] m_rowdim;
        [MirrorAnalyzed("Mcd.MatDim")]
        public readonly Dim[] m_coldim;
        [MirrorVerified("MatDimRcDimConstructionDifferentialTests")]
        private rc_dim(Dim[] r, Dim[] c) : base(r.Length, c.Length) { m_rowdim = r; m_coldim = c; }
        [MirrorVerified("MatDimRcDimConstructionDifferentialTests")]
        public static rc_dim make_rc(Dim[] r, Dim[] c) => new(r, c);
        [MirrorVerified("MatDimRcDimCoordinatesDifferentialTests")]
        public override Dim dim(int i, int j) => Dim.Mult(m_rowdim[i], m_coldim[j]);
        [MirrorVerified("MatDimRcDimCoordinatesDifferentialTests")]
        public override MatDim matdim(int i, int j) => uniform_dim.make(dim(i, j), 1, 1);
        [MirrorVerified("MatDimRcDimCoordinatesDifferentialTests")]
        public override MatDim IsLinear => this;
        [MirrorVerified("MatDimRcDimCoordinatesDifferentialTests")]
        public override Dim[] RowDimensionArray(int r) => Mult(m_rowdim[r], m_coldim);
        [MirrorVerified("MatDimRcDimCoordinatesDifferentialTests")]
        public override Dim[] ColDimensionArray(int c) => Mult(m_coldim[c], m_rowdim);
        [MirrorVerified("MatDimRcDimEqualityDifferentialTests")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(this);
        [MirrorVerified("MatDimRcDimEqualityDifferentialTests")]
        protected override bool eq2_b(rc_dim m1) => Eq_b(m_rowdim, m1.m_rowdim) & Eq_b(m_coldim, m1.m_coldim);
        [MirrorVerified("MatDimRcDimArithmeticDifferentialTests")]
        protected override MatDim mmult1(MatDim m2) => m2.mmult2(this);
        [MirrorVerified("MatDimRcDimArithmeticDifferentialTests")]
        protected override MatDim mmult2(c_dim m1)
        {
            Dim factor = Mult(m1.m_coldim, m_rowdim);
            return factor == null ? null : c_dim.make(Mult(factor, m_coldim), m1.Rows);
        }
        [MirrorVerified("MatDimRcDimArithmeticDifferentialTests")]
        protected override MatDim mmult2(rc_dim m1)
        {
            Dim factor = Mult(m1.m_coldim, m_rowdim);
            return factor == null ? null : make_rc(Mult(factor, m1.m_rowdim), m_coldim);
        }
        [MirrorVerified("MatDimRcDimArithmeticDifferentialTests")]
        protected override MatDim transpose() => make_rc(m_coldim, m_rowdim);
        [MirrorVerified("MatDimRcDimArithmeticDifferentialTests")]
        protected override Dim det(int n) => Product(Product(Dim.identity, m_rowdim), m_coldim);
        [MirrorVerified("MatDimRcDimArithmeticDifferentialTests")]
        protected override MatDim invert() => make_rc(Invert(m_coldim), Invert(m_rowdim));
        [MirrorVerified("MatDimRcDimArithmeticDifferentialTests")]
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
        [MirrorVerified("MatDimCrazyDimConstructionDifferentialTests")]
        protected crazy_dim(MatVal m) : base(m.Rows, m.Cols) => m_matrix = m;
        [MirrorVerified("MatDimCrazyDimCoordinatesDifferentialTests")]
        public override Dim dim(int i, int j) => m_matrix.get_2_dim(i, j);
        [MirrorVerified("MatDimCrazyDimCoordinatesDifferentialTests")]
        public override MatDim matdim(int i, int j) => m_matrix.get_2_matdim(i, j);
        [MirrorVerified("MatDimCrazyDimEqualityDifferentialTests")]
        protected override bool eq_b(MatDim m2) => m2.eq2_b(this);
        [MirrorVerified("MatDimCrazyDimEqualityDifferentialTests")]
        protected override bool eq2_b(crazy_dim m1)
        {
            if ((m_row != m1.m_row) | (m_col != m1.m_col)) return false;
            for (int r = 0; r < m_row; r++)
            for (int c = 0; c < m_col; c++)
                if (!Eq_b(matdim(r, c), m1.matdim(r, c))) return false;
            return true;
        }
        [MirrorVerified("MatDimCrazyDimConstructionDifferentialTests")]
        public static new crazy_dim make(MatVal m) => new(m);
    }
}
