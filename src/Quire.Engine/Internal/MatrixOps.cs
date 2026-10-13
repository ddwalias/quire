using Quire.Mcd;

namespace Quire.Engine.Internal;

/// <summary>Matrix routines over <see cref="MatVal"/>; linear algebra requires uniform units per matrix.</summary>
internal static class MatrixOps
{
    public static MatVal AsMatrix(DimVal v, string what) => Ops.Normalize(v) switch
    {
        MatVal m => m,
        NumVal n => Val.Column([n]),
        _ => throw new CalcException($"{what} must be a matrix or vector, got {Ops.Describe(v)}"),
    };

    public static (double[,] Values, Dim Dim) Uniform(MatVal m, string what)
    {
        var d = m.At(0, 0).m_dim;
        var a = new double[m.Rows, m.Cols];
        for (int r = 0; r < m.Rows; r++)
            for (int c = 0; c < m.Cols; c++)
            {
                var e = m.At(r, c);
                if (e.m_dim != d) throw new CalcException($"{what}: all elements must have the same units");
                a[r, c] = e.Real;
            }
        return (a, d);
    }

    public static MatVal FromArray(double[,] a, Dim d) =>
        Val.Create(a.GetLength(0), a.GetLength(1), (r, c) => Val.Num(a[r, c], d));

    public static MatVal Identity(int n, Dim d) => Val.Create(n, n, (r, c) => Val.Num(r == c ? 1 : 0, d));

    private static void RequireSquare(MatVal m, string what)
    {
        if (m.Rows != m.Cols) throw new CalcException($"{what} requires a square matrix (got {m.Rows}×{m.Cols})");
    }

    public static MatVal Inverse(MatVal m)
    {
        RequireSquare(m, "Inverse");
        var (a, d) = Uniform(m, "Inverse");
        int n = m.Rows;
        var id = new double[n, n];
        for (int i = 0; i < n; i++) id[i, i] = 1;
        return FromArray(Numerics.LuSolve(a, id), Ops.Pwr(d, -1));
    }

    public static NumVal Determinant(MatVal m)
    {
        RequireSquare(m, "Determinant");
        var (a, d) = Uniform(m, "Determinant");
        return Val.Num(Numerics.Determinant(a), Ops.Pwr(d, m.Rows));
    }

    public static MatVal LSolve(MatVal m, MatVal v)
    {
        RequireSquare(m, "lsolve");
        if (v.Rows != m.Rows) throw new CalcException("lsolve: vector length must match matrix rows");
        var (a, dm) = Uniform(m, "lsolve");
        var (b, dv) = Uniform(v, "lsolve");
        return FromArray(Numerics.LuSolve(a, b), Dim.Div(dv, dm));
    }

    public static MatVal Augment(IReadOnlyList<MatVal> parts)
    {
        int rows = parts[0].Rows;
        if (parts.Any(p => p.Rows != rows)) throw new CalcException("augment: matrices must have the same number of rows");
        int cols = parts.Sum(p => p.Cols);
        return Val.Create(rows, cols, (r, c) =>
        {
            foreach (var p in parts)
            {
                if (c < p.Cols) return p.At(r, c);
                c -= p.Cols;
            }
            throw new InvalidOperationException();
        });
    }

    public static MatVal Stack(IReadOnlyList<MatVal> parts)
    {
        int cols = parts[0].Cols;
        if (parts.Any(p => p.Cols != cols)) throw new CalcException("stack: matrices must have the same number of columns");
        return Augment(parts.Select(p => p.Transpose()).ToList()).Transpose();
    }
}
