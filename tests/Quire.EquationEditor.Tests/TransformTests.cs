using System.Xml.Linq;
using Quire.Calculation;
using Quire.Calculation.Core;
using Quire.Engine;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.Tree.Transform;
using Quire.Values;
using Quire.Controls.Core;

namespace Quire.EquationEditor.Tests;

public class TransformTests
{
    [Theory]
    [InlineData("x := 3")]
    [InlineData("f(x, y) := x^2 + y")]
    [InlineData("v[i] := i^2")]
    [InlineData("y := 2 x =")]
    [InlineData("v = km/hr")]
    [InlineData("k ≡ 2")]
    [InlineData("M := [1, 2, 3; 4, 5, 6]")]
    [InlineData("x := 0, 0.5 .. 10")]
    [InlineData("a == b")]
    [InlineData("|x| + sqrt(y) + n! =")]
    [InlineData("s := \"text\"")]
    [InlineData("sum(k^2, k, 1, 10) =")]
    public void Xml_round_trip_preserves_meaning(string src)
    {
        var statement = LinearParser.ParseStatement(src);
        var xml = XmlMathTransform.ToXml(statement);
        var back = XmlMathTransform.StatementFromXml(XElement.Parse(xml.ToString()));
        Assert.Equal(MathNodeToRlTransform.Transform(statement), MathNodeToRlTransform.Transform(back));
    }

    [Fact]
    public void Matrices_are_stored_column_major()
    {
        var xml = XmlMathTransform.ToXml(LinearParser.ParseExpression("[1, 2; 3, 4]"));
        Assert.Equal(["1", "3", "2", "4"], xml.Elements().Select(e => e.Value));
    }

    [Fact]
    public void Reads_math50_vocabulary_with_subscript_spans()
    {
        const string xml = """
            <ml:define xmlns:ml="http://schemas.mathsoft.com/math50">
              <ml:id labels="UNIT" xml:space="preserve"><Span xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:pw="clr-namespace:Ptc.Wpf;assembly=Ptc.Core">mm<pw:Subscript>Hg</pw:Subscript></Span></ml:id>
              <ml:apply><ml:mult /><ml:id>in_Hg</ml:id><ml:apply><ml:div /><ml:id>mm</ml:id><ml:id>in</ml:id></ml:apply></ml:apply>
            </ml:define>
            """;
        var s = XmlMathTransform.StatementFromXml(XElement.Parse(xml));
        Assert.Equal("(define mm_Hg (op_mult in_Hg (op_div mm in)))", MathNodeToRlTransform.Transform(s));
    }

    [Fact]
    public void Reads_eval_with_unit_override_and_function_definitions()
    {
        const string xml = """
            <ml:eval xmlns:ml="http://schemas.mathsoft.com/math50"><ml:id>p</ml:id><ml:unitOverride><ml:id>kPa</ml:id></ml:unitOverride></ml:eval>
            """;
        Assert.Equal("(eval p kPa)", MathNodeToRlTransform.Transform(XmlMathTransform.StatementFromXml(XElement.Parse(xml))));

        const string fn = """
            <ml:define xmlns:ml="http://schemas.mathsoft.com/math50"><ml:function><ml:id>f</ml:id><ml:boundVars><ml:id>x</ml:id></ml:boundVars></ml:function>
            <ml:apply><ml:pow /><ml:id>x</ml:id><ml:real>2</ml:real></ml:apply></ml:define>
            """;
        Assert.Equal("(define (f x) (op_power x 2))", MathNodeToRlTransform.Transform(XmlMathTransform.StatementFromXml(XElement.Parse(fn))));
    }

    [Fact]
    public void Unknown_elements_raise()
    {
        var ex = Assert.Throws<UnsupportedMathXmlException>(() =>
            XmlMathTransform.NodeFromXml(XElement.Parse("<ml:program xmlns:ml=\"urn:x\"/>")));
        Assert.Equal("program", ex.Element);
    }

    [Theory]
    [InlineData(2.5, "2.5")]
    [InlineData(1 / 3.0, "0.333")]
    [InlineData(999.9996, "1·10^3")]
    [InlineData(12345, "1.235·10^4")]
    [InlineData(0.000123, "1.23·10^-4")]
    [InlineData(-42, "-42")]
    [InlineData(0, "0")]
    public void Number_formatting(double v, string expected) =>
        Assert.Equal(expected, ResultFormatter.NumberText(v, ResultFormatOptions.Default));

    [Fact]
    public void Trailing_zeros_can_be_shown() =>
        Assert.Equal("2.500", ResultFormatter.NumberText(2.5, ResultFormatOptions.Default with { ShowTrailingZeros = true }));

    [Fact]
    public void Units_render_with_denominator()
    {
        var u = ResultFormatter.UnitExpression([new UnitPower("m", 1), new UnitPower("s", -2)]);
        Assert.Equal("m/s^2", MathNodeToLinearTransform.Write(u!));
    }

    /// <summary>Keyboard text → tree → Rl → engine → display tree, end to end.</summary>
    [Theory]
    [InlineData(new[] { "F := 2 kg * 3 m/s^2", "F =" }, "6 N")]
    [InlineData(new[] { "v := 100 km/hr", "v = mph" }, "62.137")]
    [InlineData(new[] { "a := 1 ft", "a =" }, "0.305 m")]
    [InlineData(new[] { "x := [1; 2]", "x =" }, "[1; 2]")]
    [InlineData(new[] { "P := 1 atm", "P =" }, "1.013·10^5 Pa")]
    public void End_to_end_display(string[] lines, string expected)
    {
        var engine = EngineFactory.CreateEngine();
        int h = 0;
        foreach (var line in lines)
            h = engine.AddRegionAfter(h, MathNodeToRlTransform.Transform(LinearParser.ParseStatement(line)));
        var result = ValueToIResultNodeTransform.Transform(engine.GetResult(h)!, UnitSystem.SI);
        Assert.Null(result.Error);
        var tree = ResultFormatter.Instance.Format(result.Value, ResultFormatOptions.Default)!;
        Assert.Equal(expected, MathNodeToLinearTransform.Write(tree).Replace("*10^", "·10^"));
    }
}

public class ReviewRegressionTransformTests
{
    private static string Rl(MathExpr n) => MathNodeToRlTransform.Expr(n);

    [Fact]
    public void Implicit_multiplication_by_negation_or_abs_round_trips()
    {
        foreach (var right in new MathExpr[] { new UnaryNode("neg", new IdNode("x")), new CallNode("abs", [new IdNode("x")]) })
        {
            var tree = new BinaryNode("mult", new NumberNode("2"), right, Implicit: true);
            var text = MathNodeToLinearTransform.Write(tree);
            Assert.Equal(Rl(tree), Rl(LinearParser.ParseExpression(text)));
        }
    }

    [Fact]
    public void Chained_comparison_is_parenthesised()
    {
        var tree = new BinaryNode("eq", new BinaryNode("eq", new IdNode("a"), new IdNode("b")), new IdNode("c"));
        var text = MathNodeToLinearTransform.Write(tree);
        Assert.Equal(Rl(tree), Rl(LinearParser.ParseExpression(text)));
    }

    [Fact]
    public void Unparsed_statements_survive_xml_and_send_nothing()
    {
        var s = new UnparsedStatement("x + y ==");
        var back = XmlMathTransform.StatementFromXml(XElement.Parse(XmlMathTransform.ToXml(s).ToString()));
        Assert.Equal(s, back);
        Assert.Equal("(noop)", MathNodeToRlTransform.Transform(back));
    }

    [Fact]
    public void Lambda_without_bound_variables_is_unsupported_not_a_crash() =>
        Assert.Throws<UnsupportedMathXmlException>(() => XmlMathTransform.NodeFromXml(XElement.Parse(
            "<ml:apply xmlns:ml=\"urn:x\"><ml:summation/><ml:lambda><ml:id>k</ml:id></ml:lambda></ml:apply>")));
}

public class LinearWriterEdgeTests
{
    [Fact]
    public void Implicit_multiplication_by_power_of_abs_round_trips()
    {
        var tree = new BinaryNode("mult", new NumberNode("2"),
            new BinaryNode("pow", new CallNode("abs", [new IdNode("x")]), new NumberNode("2")), Implicit: true);
        var text = MathNodeToLinearTransform.Write(tree);
        Assert.Equal(MathNodeToRlTransform.Expr(tree), MathNodeToRlTransform.Expr(LinearParser.ParseExpression(text)));
    }
}
