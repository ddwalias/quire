namespace Quire.Engine;

/// <summary>Plain-double numerical routines; callers handle units.</summary>
public static class Numerics
{
    /// <summary>Solves A·X = B with partial pivoting. Throws <see cref="CalcException"/> when singular.</summary>
    public static double[,] LuSolve(double[,] a, double[,] b)
    {
        int n = a.GetLength(0), m = b.GetLength(1);
        var M = new double[n, n + m];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++) M[i, j] = a[i, j];
            for (int j = 0; j < m; j++) M[i, n + j] = b[i, j];
        }
        double scale = 0;
        foreach (var v in a) scale = Math.Max(scale, Math.Abs(v));
        for (int k = 0; k < n; k++)
        {
            int p = k;
            for (int i = k + 1; i < n; i++) if (Math.Abs(M[i, k]) > Math.Abs(M[p, k])) p = i;
            if (Math.Abs(M[p, k]) <= 1e-14 * Math.Max(scale, 1e-300)) throw new CalcException("Matrix is singular");
            if (p != k) for (int j = 0; j < n + m; j++) (M[k, j], M[p, j]) = (M[p, j], M[k, j]);
            for (int i = k + 1; i < n; i++)
            {
                double f = M[i, k] / M[k, k];
                if (f == 0) continue;
                for (int j = k; j < n + m; j++) M[i, j] -= f * M[k, j];
            }
        }
        var X = new double[n, m];
        for (int c = 0; c < m; c++)
            for (int i = n - 1; i >= 0; i--)
            {
                double s = M[i, n + c];
                for (int j = i + 1; j < n; j++) s -= M[i, j] * X[j, c];
                X[i, c] = s / M[i, i];
            }
        return X;
    }

    public static double Determinant(double[,] a)
    {
        int n = a.GetLength(0);
        var M = (double[,])a.Clone();
        double d = 1;
        for (int k = 0; k < n; k++)
        {
            int p = k;
            for (int i = k + 1; i < n; i++) if (Math.Abs(M[i, k]) > Math.Abs(M[p, k])) p = i;
            if (M[p, k] == 0) return 0;
            if (p != k) { for (int j = 0; j < n; j++) (M[k, j], M[p, j]) = (M[p, j], M[k, j]); d = -d; }
            d *= M[k, k];
            for (int i = k + 1; i < n; i++)
            {
                double f = M[i, k] / M[k, k];
                for (int j = k; j < n; j++) M[i, j] -= f * M[k, j];
            }
        }
        return d;
    }

    // Gauss–Kronrod 7/15 nodes and weights.
    private static readonly double[] Xgk = [0.991455371120813, 0.949107912342759, 0.864864423359769, 0.741531185599394,
        0.586087235467691, 0.405845151377397, 0.207784955007898, 0];
    private static readonly double[] Wgk = [0.022935322010529, 0.063092092629979, 0.104790010322250, 0.140653259715525,
        0.169004726639267, 0.190350578064785, 0.204432940075298, 0.209482141084728];
    private static readonly double[] Wg = [0.129484966168870, 0.279705391489277, 0.381830050505119, 0.417959183673469];

    private static (double Value, double Error) Gk15(Func<double, double> f, double a, double b)
    {
        double c = (a + b) / 2, h = (b - a) / 2, fc = f(c);
        double k = fc * Wgk[7], g = fc * Wg[3];
        for (int j = 0; j < 7; j++)
        {
            double dx = h * Xgk[j], s = f(c - dx) + f(c + dx);
            k += Wgk[j] * s;
            if (j % 2 == 1) g += Wg[(j - 1) / 2] * s;
        }
        return (k * h, Math.Abs((k - g) * h));
    }

    /// <summary>Adaptive Gauss–Kronrod quadrature; infinite limits are mapped onto a finite interval.</summary>
    public static double Integrate(Func<double, double> f, double a, double b, double tol = 1e-10)
    {
        if (a == b) return 0;
        if (a > b) return -Integrate(f, b, a, tol);
        if (double.IsInfinity(a) || double.IsInfinity(b))
        {
            // x = t / (1 - t²), dx = (1 + t²) / (1 - t²)² dt
            double G(double t)
            {
                if (Math.Abs(t) >= 1) return 0;
                double u = 1 - t * t;
                return f(t / u) * (1 + t * t) / (u * u);
            }
            double ta = double.IsInfinity(a) ? -1 : InvMap(a), tb = double.IsInfinity(b) ? 1 : InvMap(b);
            return Integrate(G, ta, tb, tol);
        }
        double total = 0;
        int evals = 0;
        var whole = Gk15(f, a, b).Value;
        double scale = Math.Max(Math.Abs(whole), 1e-300);
        var stack = new Stack<(double Lo, double Hi, int Depth)>();
        stack.Push((a, b, 0));
        while (stack.Count > 0)
        {
            var (lo, hi, depth) = stack.Pop();
            var (val, err) = Gk15(f, lo, hi);
            evals += 15;
            if (err <= tol * scale * (hi - lo) / (b - a) || depth > 40 || evals > 400_000) total += val;
            else
            {
                double mid = (lo + hi) / 2;
                stack.Push((lo, mid, depth + 1));
                stack.Push((mid, hi, depth + 1));
            }
        }
        if (double.IsNaN(total)) throw new CalcException("Integral did not converge");
        return total;
    }

    private static double InvMap(double x) => x == 0 ? 0 : (-1 + Math.Sqrt(1 + 4 * x * x)) / (2 * x);

    /// <summary>Five-point central difference; higher orders by nesting.</summary>
    public static double Derivative(Func<double, double> f, double x, int order = 1)
    {
        if (order == 0) return f(x);
        if (order > 1) return Derivative(t => Derivative(f, t, order - 1), x);
        double h = 1e-3 * Math.Max(Math.Abs(x), 1e-2);
        return (-f(x + 2 * h) + 8 * f(x + h) - 8 * f(x - h) + f(x - 2 * h)) / (12 * h);
    }

    /// <summary>Secant iteration from a guess, switching to Brent's method once a sign change is bracketed.</summary>
    public static double FindRoot(Func<double, double> f, double guess)
    {
        double x1 = guess, x2 = guess == 0 ? 1e-3 : guess * 1.001;
        double f1 = f(x1), f2 = f(x2);
        if (f1 == 0) return x1;
        for (int i = 0; i < 200; i++)
        {
            if (f2 == 0) return x2;
            if (f1 * f2 < 0) return Brent(f, Math.Min(x1, x2), Math.Max(x1, x2));
            double x3 = x2 - f2 * (x2 - x1) / (f2 - f1);
            if (!double.IsFinite(x3)) break;
            x1 = x2; f1 = f2; x2 = x3; f2 = f(x2);
            if (Math.Abs(x2 - x1) <= 1e-14 * Math.Max(1, Math.Abs(x2)) && Math.Abs(f2) < 1e-9 * Math.Max(1, Math.Abs(f1))) return x2;
        }
        throw new CalcException("Root not found; try another guess or a bracket root(f, x, a, b)");
    }

    public static double Brent(Func<double, double> f, double a, double b, double tol = 1e-15)
    {
        double fa = f(a), fb = f(b);
        if (fa == 0) return a;
        if (fb == 0) return b;
        if (fa * fb > 0) throw new CalcException("root: f(a) and f(b) must have opposite signs");
        double c = a, fc = fa, d = b - a, e = d;
        for (int i = 0; i < 300; i++)
        {
            if (fb * fc > 0) { c = a; fc = fa; d = e = b - a; }
            if (Math.Abs(fc) < Math.Abs(fb)) { a = b; b = c; c = a; fa = fb; fb = fc; fc = fa; }
            double tol1 = 2 * double.Epsilon + 2e-16 * Math.Abs(b) + tol / 2, xm = (c - b) / 2;
            if (Math.Abs(xm) <= tol1 || fb == 0) return b;
            if (Math.Abs(e) >= tol1 && Math.Abs(fa) > Math.Abs(fb))
            {
                double s = fb / fa, p, q;
                if (a == c) { p = 2 * xm * s; q = 1 - s; }
                else
                {
                    double qa = fa / fc, r = fb / fc;
                    p = s * (2 * xm * qa * (qa - r) - (b - a) * (r - 1));
                    q = (qa - 1) * (r - 1) * (s - 1);
                }
                if (p > 0) q = -q; else p = -p;
                if (2 * p < Math.Min(3 * xm * q - Math.Abs(tol1 * q), Math.Abs(e * q))) { e = d; d = p / q; }
                else { d = xm; e = d; }
            }
            else { d = xm; e = d; }
            a = b; fa = fb;
            b += Math.Abs(d) > tol1 ? d : (xm > 0 ? tol1 : -tol1);
            fb = f(b);
        }
        return b;
    }

    /// <summary>Levenberg–Marquardt least squares minimising Σ r(x)².</summary>
    public static double[] LevenbergMarquardt(Func<double[], double[]> residuals, double[] x0, int maxIter = 400)
    {
        var x = (double[])x0.Clone();
        var r = residuals(x);
        double cost = SumSq(r), lambda = 1e-3;
        int n = x.Length;
        for (int it = 0; it < maxIter && cost > 1e-30; it++)
        {
            var J = new double[r.Length, n];
            for (int j = 0; j < n; j++)
            {
                double h = 1.5e-8 * Math.Max(Math.Abs(x[j]), 1e-3);
                var xp = (double[])x.Clone();
                xp[j] += h;
                var rp = residuals(xp);
                for (int i = 0; i < r.Length; i++) J[i, j] = (rp[i] - r[i]) / h;
            }
            var JtJ = new double[n, n];
            var Jtr = new double[n, 1];
            for (int a = 0; a < n; a++)
            {
                for (int b = 0; b < n; b++)
                {
                    double s = 0;
                    for (int i = 0; i < r.Length; i++) s += J[i, a] * J[i, b];
                    JtJ[a, b] = s;
                }
                double t = 0;
                for (int i = 0; i < r.Length; i++) t += J[i, a] * r[i];
                Jtr[a, 0] = -t;
            }
            bool improved = false;
            for (int tries = 0; tries < 25; tries++)
            {
                var A = (double[,])JtJ.Clone();
                for (int a = 0; a < n; a++) A[a, a] += lambda * Math.Max(JtJ[a, a], 1e-12);
                double[,] step;
                try { step = LuSolve(A, Jtr); }
                catch (CalcException) { lambda *= 10; continue; }
                var xn = new double[n];
                for (int j = 0; j < n; j++) xn[j] = x[j] + step[j, 0];
                var rn = residuals(xn);
                double cn = SumSq(rn);
                if (double.IsFinite(cn) && cn < cost)
                {
                    bool tiny = true;
                    for (int j = 0; j < n; j++) tiny &= Math.Abs(step[j, 0]) <= 1e-15 * Math.Max(Math.Abs(x[j]), 1e-300);
                    x = xn; r = rn; cost = cn;
                    lambda = Math.Max(lambda / 10, 1e-15);
                    improved = true;
                    if (tiny) return x;
                    break;
                }
                lambda *= 10;
            }
            if (!improved) break;
        }
        return x;
    }

    private static double SumSq(double[] r) => r.Sum(v => v * v);

    /// <summary>Nelder–Mead simplex minimisation.</summary>
    public static double[] NelderMead(Func<double[], double> f, double[] x0, int maxIter = 5000)
    {
        int n = x0.Length;
        var pts = new List<(double[] P, double V)> { (x0, f(x0)) };
        for (int i = 0; i < n; i++)
        {
            var p = (double[])x0.Clone();
            p[i] = p[i] != 0 ? p[i] * 1.05 : 2.5e-4;
            pts.Add((p, f(p)));
        }
        for (int it = 0; it < maxIter; it++)
        {
            pts.Sort((a, b) => a.V.CompareTo(b.V));
            var best = pts[0];
            var worst = pts[n];
            bool flat = Math.Abs(worst.V - best.V) <= 1e-14 * (Math.Abs(best.V) + 1e-300);
            bool tight = pts.All(s => s.P.Select((x, j) => Math.Abs(x - best.P[j]) <= 1e-11 * Math.Max(1, Math.Abs(x))).All(t => t));
            if (flat && tight) break;
            var centroid = new double[n];
            for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) centroid[j] += pts[i].P[j] / n;
            double[] At(double t) => centroid.Select((c, j) => c + t * (worst.P[j] - c)).ToArray();
            var r = At(-1);
            double fr = f(r);
            if (fr < best.V)
            {
                var e = At(-2);
                double fe = f(e);
                pts[n] = fe < fr ? (e, fe) : (r, fr);
            }
            else if (fr < pts[n - 1].V) pts[n] = (r, fr);
            else
            {
                var c = At(fr < worst.V ? -0.5 : 0.5);
                double fc = f(c);
                if (fc < Math.Min(fr, worst.V)) pts[n] = (c, fc);
                else
                    for (int i = 1; i <= n; i++)
                    {
                        var p = pts[i].P.Select((x, j) => best.P[j] + (x - best.P[j]) / 2).ToArray();
                        pts[i] = (p, f(p));
                    }
            }
        }
        pts.Sort((a, b) => a.V.CompareTo(b.V));
        return pts[0].P;
    }

    /// <summary>Natural cubic spline second derivatives.</summary>
    public static double[] SplineCoefficients(double[] xs, double[] ys)
    {
        int n = xs.Length;
        var y2 = new double[n];
        var u = new double[n];
        for (int i = 1; i < n - 1; i++)
        {
            double sig = (xs[i] - xs[i - 1]) / (xs[i + 1] - xs[i - 1]);
            double p = sig * y2[i - 1] + 2;
            y2[i] = (sig - 1) / p;
            u[i] = (ys[i + 1] - ys[i]) / (xs[i + 1] - xs[i]) - (ys[i] - ys[i - 1]) / (xs[i] - xs[i - 1]);
            u[i] = (6 * u[i] / (xs[i + 1] - xs[i - 1]) - sig * u[i - 1]) / p;
        }
        for (int k = n - 2; k >= 0; k--) y2[k] = y2[k] * y2[k + 1] + u[k];
        return y2;
    }

    public static double SplineEval(double[] xs, double[] ys, double[] y2, double x)
    {
        int lo = 0, hi = xs.Length - 1;
        while (hi - lo > 1)
        {
            int k = (hi + lo) >> 1;
            if (xs[k] > x) hi = k; else lo = k;
        }
        double h = xs[hi] - xs[lo], a = (xs[hi] - x) / h, b = (x - xs[lo]) / h;
        return a * ys[lo] + b * ys[hi] + ((a * a * a - a) * y2[lo] + (b * b * b - b) * y2[hi]) * h * h / 6;
    }
}
