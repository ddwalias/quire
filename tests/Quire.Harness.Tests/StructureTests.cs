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
    private static int Throws(int x) => x > 2 ? Left() : throw new ArgumentException("x");

    private static readonly PEReader Pe = new(File.OpenRead(typeof(StructureTests).Assembly.Location));
    private static Structure Read(string name, Func<string, string>? typeName = null)
    {
        var md = Pe.GetMetadataReader();
        var handle = md.MethodDefinitions.Single(h => md.GetString(md.GetMethodDefinition(h).Name) == name
            && SigProvider.FullName(md, md.GetMethodDefinition(h).GetDeclaringType()) == typeof(StructureTests).FullName);
        return new StructureReader(md, Pe, new SigProvider("Quire.Harness.Tests"), typeName ?? (n => n), _ => null).Read(handle);
    }

    [Fact]
    public void Locals_and_expression_bodies_do_not_count() => Assert.Empty(Read("WithLocal").Compare(Read("Inline")));

    /// <summary>`if`/`return` and `?:` lay out their arms in opposite order, which identifies the original form.</summary>
    [Fact]
    public void Branch_layout_is_part_of_the_structure() =>
        Assert.Contains(Read("IfElse").Compare(Read("Ternary")), d => d.StartsWith("calls"));

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

    [Fact]
    public void Quire_types_are_named_after_their_Mathcad_binding()
    {
        var mapped = Read("Ternary", n => n == typeof(StructureTests).FullName ? "Mcd.Fixture" : n);
        Assert.Contains("call Mcd.Fixture::Left`0()->System.Int32", mapped.Calls);
    }
}
