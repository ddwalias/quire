using Quire.Engine.Internal;
using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Engine.Tests;

public class EvaluationTests : EngineTestBase
{
    [Fact]
    public void Arithmetic_follows_operator_semantics() =>
        AssertNum(Last("(eval (op_add 1 (op_mult 2 (op_power 3 2))))"), 19);

    [Fact]
    public void Definitions_are_visible_only_below() =>
        Assert.Collection(Run("(eval x)", "(define x 3)", "(eval x)"),
            r => AssertError(r, "'x' is not defined"),
            _ => { },
            r => AssertNum(r, 3));

    [Fact]
    public void Redefinition_changes_value_for_later_regions()
    {
        var rs = Run("(define a 1)", "(eval a)", "(define a 2)", "(eval a)");
        AssertNum(rs[1], 1);
        AssertNum(rs[3], 2);
    }

    [Fact]
    public void Global_definitions_are_visible_above() =>
        AssertNum(Run("(eval (op_mult k 2))", "(global k 21)")[0], 42);

    [Fact]
    public void User_functions_capture_definitions_at_their_location()
    {
        var rs = Run("(define a 10)", "(define (f x) (op_add x a))", "(define a 100)", "(eval (op_apply f 1))");
        AssertNum(rs[3], 11);
    }

    [Fact]
    public void Recursive_functions_work() =>
        AssertNum(Last(
            "(define (fib n) (op_apply if (op_lt n 2) n (op_add (op_apply fib (op_sub n 1)) (op_apply fib (op_sub n 2)))))",
            "(eval (op_apply fib 15))"), 610);

    [Fact]
    public void Runaway_recursion_reports_an_error() =>
        AssertError(Last("(define (f n) (op_apply f n))", "(eval (op_apply f 1))"), "Recursion is too deep");

    [Fact]
    public void Wrong_argument_count_is_reported() =>
        AssertError(Last("(define (f x y) (op_add x y))", "(eval (op_apply f 1))"), "expects 2");

    [Fact]
    public void Builtins_cover_elementary_functions()
    {
        AssertNum(Last("(eval (op_apply sin (op_div π 2)))"), 1);
        AssertNum(Last("(eval (op_apply ln e))"), 1);
        AssertNum(Last("(eval (op_apply log 1000))"), 3);
        AssertNum(Last("(eval (op_factorial 5))"), 120);
        AssertNum(Last("(eval (op_rootn 3 -27))"), -3);
    }

    [Fact]
    public void Strings_concatenate() =>
        Assert.Equal("ab", Assert.IsType<StrVal>(Last("(eval (op_apply concat \"a\" \"b\"))").Value).Field1);

    [Fact]
    public void Undefined_symbol_names_the_symbol()
    {
        var r = Last("(eval (op_add zz 1))");
        Assert.Equal("zz", r.Error!.Symbol);
    }

    [Fact]
    public void Redefining_a_unit_warns() =>
        Assert.Contains("unit m", Last("(define m 5)").Warning);

    [Fact]
    public void Malformed_rl_is_reported_per_node() =>
        Assert.NotNull(Last("(eval (op_add 1 2").Error);
}

/// <summary>Regressions found in code review.</summary>
public class ReviewRegressionTests : EngineTestBase
{
    [Fact]
    public void Min_and_sec_are_time_units_in_value_position()
    {
        AssertNum(Last("(eval (op_mult 5 min))"), 300, Dim.Parse(BuiltinUnits.Time));
        AssertNum(Last("(eval (op_mult 2 sec))"), 2, Dim.Parse(BuiltinUnits.Time));
        AssertNum(Last("(eval (op_apply min 3 1 2))"), 1);
    }

    [Fact]
    public void Affine_temperature_applies_to_unitless_expressions()
    {
        AssertNum(Last("(define t 20)", "(eval (op_mult t °C))"), 293.15, Dim.Parse(BuiltinUnits.Temperature));
        AssertNum(Last("(eval (op_mult (op_add 20 5) °C))"), 298.15, Dim.Parse(BuiltinUnits.Temperature));
    }

    [Fact]
    public void Affine_unit_after_dimensioned_operand_is_a_difference() =>
        AssertNum(Last("(eval (op_div J (op_mult kg °C)))"), 1, Dim.Parse("V{Length 120000 Temperature-60000 Time-120000}"));

    [Fact]
    public void Derivative_iterates_a_range_variable()
    {
        var r = Last("(define x (op_range2 0 3))", "(eval (op_deriv_1 (op_power x 2) x))");
        Assert.True(r.IsRangeTable);
        var m = Assert.IsType<MatVal>(r.Value);
        Assert.Equal([0.0, 2, 4, 6], m.Elements().Select(e => Math.Round(e.Real, 6)));
    }

    [Fact]
    public void Indexed_definition_pads_with_matching_units()
    {
        AssertNum(Last("(define (op_index v 2) (op_mult 5 m))", "(eval (op_apply sum v))"), 5, Dim.Parse(BuiltinUnits.Length));
    }

    [Fact]
    public void Huge_matrices_are_errors_not_crashes() =>
        AssertError(Last("(eval (op_apply identity 40000))"), "too large");

    [Fact]
    public void Moving_a_node_relative_to_itself_or_an_unknown_node_keeps_it()
    {
        int a = Engine.AddAfter(0, "(define a 1)", 0, 0);
        int b = Engine.AddAfter(a, "(eval a)", 0, 0);
        Engine.MoveAfter(b, b, 0);
        Assert.Throws<ArgumentException>(() => Engine.MoveAfter(b, 999, 0));
        Assert.Equal([a, b], Engine.Handles);
        Assert.Equal(1, ((NumVal)Engine.GetResult(b)!.Value!).Real);
    }
}

public class SizeLimitTests : EngineTestBase
{
    [Fact]
    public void Huge_linspace_is_rejected_quickly() =>
        AssertError(Last("(eval (op_apply linspace 0 1 400000000))"), "too many");
}
