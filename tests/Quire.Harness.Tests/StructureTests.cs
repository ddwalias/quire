using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Quire.Tools.MirrorGen;

namespace Quire.Harness.Tests;

public class StructureTests
{
    private static int Left() => 1;
    private static int Right() => 2;
    private static int IfElse(int x)
    {
        if (x > 2) return Left();
        return Right();
    }
    private static int Ternary(int x) => x > 2 ? Left() : Right();
    private static int Inline(int x) => Left() + x * 3;
    private static int WithLocal(int x)
    {
        int scaled = x * 3;
        return Left() + scaled;
    }
    private static int Swapped(int x) => x > 2 ? Right() : Left();
    private static int Conjunction(int x) => x > 2 && x < 9 ? Left() : Right();
    private static int Constant(int x) => x > 3 ? Left() : Right();
    private static int CatchArgument() { try { return Left(); } catch (ArgumentException) { return Right(); } }
    private static int CatchOperation() { try { return Left(); } catch (InvalidOperationException) { return Right(); } }
    private static int Pick(int x, int y)
    {
        if (x > 2) return y;
        return x;
    }
    private static int PickAgain(int x, int y)
    {
        if (x > 2) return y;
        return x;
    }
    private static int PickTernary(int x, int y) => x <= 2 ? x : y;
    private static int Difference(int x, int y)
    {
        int a = x * 3, b = y * 5;
        return a - b;
    }
    private static int DifferenceSwapped(int x, int y)
    {
        int a = x * 3, b = y * 5;
        return b - a;
    }
    private static int Throws(int x) => x > 2 ? Left() : throw new ArgumentException("x");
    private static readonly PEReader Pe = new(File.OpenRead(typeof(StructureTests).Assembly.Location));
    private static Structure Read(string name)
    {
        var md = Pe.GetMetadataReader();
        var handle = md.MethodDefinitions.Single(h => md.GetString(md.GetMethodDefinition(h).Name) == name
            && SigProvider.FullName(md, md.GetMethodDefinition(h).GetDeclaringType()) == typeof(StructureTests).FullName);
        return new StructureReader(md, Pe, new SigProvider("Quire.Harness.Tests"), _ => null).Read(handle);
    }

    /// <summary>il_diff.py's output: the first differing instruction with context, both sides, each Quire instruction
    /// with the source line of its IL offset; nothing for an exact match.</summary>
    [Fact]
    public void Diff_shows_the_first_differing_instruction_with_context()
    {
        Assert.Empty(Read("Pick").Diff(Read("PickAgain"), 3, _ => "x"));
        Structure quire = Read("IfElse"), mathcad = Read("Ternary");
        Assert.Equal(quire.Code.Count(line => !line.StartsWith('.')), quire.Offsets.Length);
        var lines = quire.Diff(mathcad, 1, offset => $"at {offset}");
        int at = Enumerable.Range(0, quire.Code.Length).First(i => quire.Code[i] != mathcad.Code[i]);
        Assert.Equal($"first difference at instruction #{at} (Quire | Mathcad  [Quire source line])", lines[0]);
        var marked = Assert.Single(lines, l => l.StartsWith('>'));
        Assert.StartsWith($"> #{at} ", marked);
        Assert.Matches($@" {System.Text.RegularExpressions.Regex.Escape(quire.Code[at])} +\| ", marked);
        Assert.EndsWith($"| {mathcad.Code[at]}  [at {quire.Offsets[at]}]", marked);
        Assert.Equal(Math.Min(at, 1) + 2, lines.Count - 1);  // one instruction either side, fewer at the start
    }

    [Fact]
    public void Locals_and_expression_bodies_do_not_count() => Assert.Empty(Read("WithLocal").Compare(Read("Inline")));

    /// <summary>`if`/`return` and `?:` lay out their arms in opposite order, which identifies the original form.</summary>
    [Fact]
    public void Branch_layout_is_part_of_the_structure() =>
        Assert.Contains(Read("IfElse").Compare(Read("Ternary")), d => d.StartsWith("calls"));

    /// <summary>Which local an instruction reads counts.</summary>
    [Fact]
    public void An_exact_match_keeps_which_local_is_read() =>
        Assert.False(Read("Difference").Exact(Read("DifferenceSwapped")));

    [Fact]
    public void An_exact_match_keeps_branch_direction_and_block_order()
    {
        Assert.True(Read("Pick").Exact(Read("PickAgain")));
        // Release optimization gives these two source forms identical IL.
        Assert.True(Read("Pick").Exact(Read("PickTernary")));
        Assert.False(Read("IfElse").Exact(Read("Ternary")));
    }

    [Theory]
    [InlineData("Swapped", "calls")]
    [InlineData("Conjunction", "conditional branches")]
    [InlineData("Constant", "constants")]
    [InlineData("CatchOperation", "exception regions")]
    [InlineData("Throws", "throws")]
    public void Logic_changes_are_reported(string variant, string difference)
    {
        var baseline = variant.StartsWith("Catch") ? "CatchArgument" : "Ternary";
        Assert.Contains(Read(variant).Compare(Read(baseline)), d => d.StartsWith(difference));
    }

    private static Uri OwnResource() => new("/Quire.Harness.Tests;component/table.xaml", UriKind.Relative);
    private static Uri OtherResource() => new("/MathcadPrime;component/table.xaml", UriKind.Relative);

    /// <summary>D21: a pack URI naming the assembly that holds the literal compares by role, not by name.</summary>
    [Fact]
    public void Own_assembly_component_uris_compare_as_self()
    {
        Assert.Equal("/<self>;component/a.xaml", StructureReader.OwnComponent("Ptc.Calculation.Core", "/Ptc.Calculation.Core;component/a.xaml"));
        Assert.Equal("/<self>;component/a.xaml", StructureReader.OwnComponent("Quire.Calculation.Core", "/quire.calculation.core;component/a.xaml"));
        Assert.Equal("pack://application:,,,/<self>;v1.0.0.0;component/a.xaml",
            StructureReader.OwnComponent("Ptc.Core", "pack://application:,,,/Ptc.Core;v1.0.0.0;component/a.xaml"));
        Assert.Equal("/<self>;0123456789abcdef;component/a.xaml", StructureReader.OwnComponent("Ptc.Core", "/Ptc.Core;0123456789abcdef;component/a.xaml"));
        Assert.Equal("/<self>;v1.0.0.0;0123456789abcdef;component/a.xaml",
            StructureReader.OwnComponent("Ptc.Core", "/Ptc.Core;v1.0.0.0;0123456789abcdef;component/a.xaml"));
        Assert.Equal("/Ptc.Core;v;component/a.xaml", StructureReader.OwnComponent("Ptc.Core", "/Ptc.Core;v;component/a.xaml"));
        Assert.Equal("/Ptc.Core;component/a.xaml", StructureReader.OwnComponent("Ptc.Calculation.Core", "/Ptc.Core;component/a.xaml"));
        Assert.Equal("/Ptc.CoreX;component/a.xaml", StructureReader.OwnComponent("Ptc.Core", "/Ptc.CoreX;component/a.xaml"));
        Assert.Equal("see /Ptc.Core;component/a.xaml", StructureReader.OwnComponent("Ptc.Core", "see /Ptc.Core;component/a.xaml"));
        Assert.Equal("/Ptc.Core;other/a.xaml", StructureReader.OwnComponent("Ptc.Core", "/Ptc.Core;other/a.xaml"));
        Assert.Contains("ldstr str /<self>;component/table.xaml", Read(nameof(OwnResource)).Code);
        Assert.Contains("ldstr str /MathcadPrime;component/table.xaml", Read(nameof(OtherResource)).Code);
    }
}
