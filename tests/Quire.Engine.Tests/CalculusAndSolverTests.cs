using Quire.Engine.Internal;
using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Engine.Tests;

public class CalculusAndSolverTests : EngineTestBase
{
    [Fact]
    public void Summation_over_integer_limits() =>
        AssertNum(Last("(eval (op_sigmasum (op_power k 2) k 1 10))"), 385);

    [Fact]
    public void Summation_over_a_range_variable() =>
        AssertNum(Last("(define i (op_range2 1 4))", "(eval (op_apply sum (op_mult 2 i) i))"), 20);

    [Fact]
    public void Product_over_integer_limits() =>
        AssertNum(Last("(eval (op_sigmaproduct k k 1 5))"), 120);

    [Fact]
    public void Definite_integral() =>
        AssertNum(Last("(eval (op_integral (op_power x 2) x 0 3))"), 9, relTol: 1e-9);

    [Fact]
    public void Integral_with_units_multiplies_dimensions() =>
        AssertNum(Last("(eval (op_integral (op_mult 2 N) x (op_mult 0 m) (op_mult 3 m)))"), 6, Dim.Parse(BuiltinUnits.Energy));

    [Fact]
    public void Improper_integral() =>
        AssertNum(Last("(eval (op_integral (op_apply exp (op_negate (op_power x 2))) x (op_negate ∞) ∞))"), Math.Sqrt(Math.PI), relTol: 1e-7);

    [Fact]
    public void Derivative_at_current_value() =>
        AssertNum(Last("(define x 2)", "(eval (op_deriv_1 (op_power x 3) x))"), 12, relTol: 1e-7);

    [Fact]
    public void Second_derivative() =>
        AssertNum(Last("(define x 1)", "(eval (op_deriv_n (op_apply sin x) x 2))"), -Math.Sin(1), relTol: 1e-5);

    [Fact]
    public void Root_from_guess_and_bracket()
    {
        AssertNum(Last("(define x 1)", "(eval (op_apply root (op_sub (op_power x 2) 2) x))"), Math.Sqrt(2), relTol: 1e-10);
        AssertNum(Last("(eval (op_apply root (op_sub (op_apply cos x) x) x 0 1))"), 0.7390851332151607, relTol: 1e-10);
    }

    [Fact]
    public void Root_keeps_units() =>
        AssertNum(Last("(define L (op_mult 1 m))", "(eval (op_apply root (op_sub (op_power L 2) (op_mult 9 (op_power m 2))) L))"), 3, Dim.Parse("length"), 1e-9);

    [Fact]
    public void Solve_block_find_solves_nonlinear_system()
    {
        var r = Last("(solveblock (guesses (define x 1) (define y 1)) " +
                     "(constraints (op_eq (op_add (op_power x 2) (op_power y 2)) 25) (op_eq (op_sub x y) 1)) " +
                     "(solver (define s (op_apply find x y))))",
                     "(eval s)");
        var s = Assert.IsType<MatVal>(r.Value);
        Assert.Equal(4, s.At(0).Real, 8);
        Assert.Equal(3, s.At(1).Real, 8);
    }

    [Fact]
    public void Solve_block_exports_only_solver_definitions()
    {
        var rs = Run("(solveblock (guesses (define z 1)) (constraints (op_eq (op_mult 2 z) 8)) (solver (define zs (op_apply find z))))",
                     "(eval zs)", "(eval z)");
        AssertNum(rs[1], 4, relTol: 1e-8);
        AssertError(rs[2], "not defined");
    }

    [Fact]
    public void Solve_block_with_units()
    {
        var r = Last("(solveblock (guesses (define F (op_mult 1 N))) " +
                     "(constraints (op_eq F (op_mult (op_mult 2 kg) (op_mult 3 (op_div m (op_power s 2)))))) " +
                     "(solver (define Fs (op_apply find F))))", "(eval Fs)");
        AssertNum(r, 6, Dim.Parse(BuiltinUnits.Force), 1e-8);
    }

    [Fact]
    public void Find_reports_no_solution()
    {
        var r = Last("(solveblock (guesses (define x 1)) (constraints (op_eq (op_power x 2) -1)) (solver (define xs (op_apply find x))))");
        Assert.True(r.HasError);
        AssertError(r.Children![^1], "No solution");
    }

    [Fact]
    public void Minerr_returns_best_fit_when_no_exact_solution()
    {
        var r = Last("(solveblock (guesses (define x 1)) (constraints (op_eq x 1) (op_eq x 3)) (solver (define xm (op_apply minerr x))))", "(eval xm)");
        AssertNum(r, 2, relTol: 1e-6);
    }

    [Fact]
    public void Inequality_constraints_are_honoured()
    {
        var r = Last("(solveblock (guesses (define x 1) (define y 1)) (constraints (op_eq (op_add x y) 10) (op_gt x 7)) " +
                     "(solver (define p (op_apply find x y))))", "(eval p)");
        var p = Assert.IsType<MatVal>(r.Value);
        Assert.True(p.At(0).Real >= 7 - 1e-6, $"x = {p.At(0).Real}");
        Assert.Equal(10, p.At(0).Real + p.At(1).Real, 6);
    }

    [Fact]
    public void Minimize_unconstrained()
    {
        var r = Last("(define (f x y) (op_add (op_power (op_sub x 1) 2) (op_power (op_add y 2) 2)))",
                     "(define x 0)", "(define y 0)", "(eval (op_apply minimize f x y))");
        var m = Assert.IsType<MatVal>(r.Value);
        Assert.Equal(1, m.At(0).Real, 5);
        Assert.Equal(-2, m.At(1).Real, 5);
    }

    [Fact]
    public void Find_outside_solve_block_is_an_error() =>
        AssertError(Last("(define x 1)", "(eval (op_apply find x))"), "solve block");
}
