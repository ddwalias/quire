using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.EquationEditor.Tests;

public class LinearParserTests
{
    private static string Rl(string src) => MathNodeToRlTransform.Transform(LinearParser.ParseStatement(src));

    [Theory]
    [InlineData("x := 3", "(define x 3)")]
    [InlineData("x : 3", "(define x 3)")]
    [InlineData("k ≡ 2", "(global k 2)")]
    [InlineData("k ~ 2", "(global k 2)")]
    [InlineData("x =", "(eval x)")]
    [InlineData("v = km/hr", "(eval v (op_div km hr))")]
    [InlineData("y := 2 x =", "(define-eval y (op_mult 2 x))")]
    [InlineData("f(x, y) := x^2 + y", "(define (f x y) (op_add (op_power x 2) y))")]
    [InlineData("v[i] := i^2", "(define (op_index v i) (op_power i 2))")]
    [InlineData("x := 0, 0.5 .. 10", "(define x (op_range3 0 0.5 10))")]
    [InlineData("i := 1 .. 5", "(define i (op_range2 1 5))")]
    [InlineData("", "(noop)")]
    [InlineData("x^2 + y^2 == 25", "(expr (op_eq (op_add (op_power x 2) (op_power y 2)) 25))")]
    public void Statements(string src, string rl) => Assert.Equal(rl, Rl(src));

    [Theory]
    [InlineData("-x^2", "(op_negate (op_power x 2))")]
    [InlineData("2^3^2", "(op_power 2 (op_power 3 2))")]
    [InlineData("a - b - c", "(op_sub (op_sub a b) c)")]
    [InlineData("9.81 m/s^2", "(op_div (op_mult 9.81 m) (op_power s 2))")]
    [InlineData("5 kg*2", "(op_mult (op_mult 5 kg) 2)")]
    [InlineData("2(x + 1)", "(op_mult 2 (op_add x 1))")]
    [InlineData("f(x)", "(op_apply f x)")]
    [InlineData("|x - 3|", "(op_abs (op_sub x 3))")]
    [InlineData("√(x)", "(op_sqrt x)")]
    [InlineData("nthroot(27, 3)", "(op_rootn 3 27)")]
    [InlineData("Mᵀ", "(op_transpose M)")]
    [InlineData("M^-1", "(op_power M (op_negate 1))")]
    [InlineData("n!", "(op_factorial n)")]
    [InlineData("[1, 2; 3, 4]", "(matrix 2 2 1 2 3 4)")]
    [InlineData("M[1, 0]", "(op_index M 1 0)")]
    [InlineData("sum(k^2, k, 1, 10)", "(op_sigmasum (op_power k 2) k 1 10)")]
    [InlineData("integral(x^2, x, 0, 1)", "(op_integral (op_power x 2) x 0 1)")]
    [InlineData("deriv(sin(x), x)", "(op_deriv_1 (op_apply sin x) x)")]
    [InlineData("a < b and not c", "(op_and (op_lt a b) (op_not c))")]
    [InlineData("1.5e3", "1500")]
    [InlineData("20 °C", "(op_mult 20 °C)")]
    [InlineData("`km/h`", "km/h")]
    [InlineData("x_max·2", "(op_mult x_max 2)")]
    public void Expressions(string src, string rl) =>
        Assert.Equal(rl, MathNodeToRlTransform.Expr(LinearParser.ParseExpression(src)));

    [Fact]
    public void Implicit_multiplication_is_flagged()
    {
        var b = Assert.IsType<BinaryNode>(LinearParser.ParseExpression("3 m"));
        Assert.True(b.Implicit);
    }

    [Theory]
    [InlineData("x :=", "Incomplete")]
    [InlineData("3 := x", "left side")]
    [InlineData("(1 + 2", "Expected ')'")]
    [InlineData("[1, 2; 3]", "different lengths")]
    [InlineData("x # 2", "Unexpected character")]
    public void Errors_report_a_message(string src, string fragment)
    {
        var s = LinearParser.TryParseStatement(src, out var error);
        Assert.Null(s);
        Assert.Contains(fragment, error!.Message);
    }

    [Theory]
    [InlineData("x := 3")]
    [InlineData("f(x, y) := (x + y)^2/2")]
    [InlineData("v = km/hr")]
    [InlineData("a - (b - c) =")]
    [InlineData("-x^2 + 2^-1 =")]
    [InlineData("M := [1, 2; 3, 4]")]
    [InlineData("t := 0 s, 0.1 s .. 2 s")]
    [InlineData("|v| + n! + Mᵀ =")]
    [InlineData("k ≡ 5")]
    [InlineData("y := 2 x =")]
    [InlineData("sum(i^2, i, 1, 3) =")]
    public void Linear_writer_round_trips(string src)
    {
        var parsed = LinearParser.ParseStatement(src);
        var text = MathNodeToLinearTransform.Transform(parsed);
        Assert.Equal(MathNodeToRlTransform.Transform(parsed), Rl(text));
    }
}
