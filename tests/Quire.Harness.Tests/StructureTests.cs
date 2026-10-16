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
    private static int Apply(Func<int> f) => f();
    private static int SameLocalFunction()
    {
        return Apply(L) + Apply(L);
        static int L() => 1;
    }
    private static int OtherLocalFunction()
    {
        return Apply(L) + Apply(K);
        static int L() => 1;
        static int K() => 1;
    }
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
    /// <summary>Same simple name as the framework type, but defined here, so not a capacity constructor.</summary>
    private sealed class StringBuilder(int capacity) { public int Capacity => capacity; }
    private static void Capacities()
    {
        // Capacity hints: not mutated.
        _ = new System.Text.StringBuilder(256);
        _ = new List<int>(7);
        _ = new Dictionary<int, int>(9);
        _ = new System.Collections.ArrayList(1);
        _ = new System.Collections.Stack(2);
        _ = new System.Collections.Queue(3);
        _ = new Stack<int>(4);
        _ = new Queue<int>(5);
        _ = new HashSet<int>(6);
        // Mutated: not on the allowlist, not a single int32, out of range, or not the framework type.
        _ = new Random(11);
        _ = new string('x', 13);
        _ = new System.Collections.Hashtable(10);
        _ = new System.Collections.Hashtable(64, StringComparer.Ordinal);
        _ = new List<int>(-1);
        _ = new System.Collections.ArrayList(2_000_000);
        _ = new StringBuilder(12);
    }
    private static int CapacityRead() => new System.Text.StringBuilder(100).Capacity;

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

    /// <summary>Locals are numbered by first use, so which local an instruction reads still counts.</summary>
    [Fact]
    public void An_exact_match_keeps_which_local_is_read() =>
        Assert.False(Read("Difference").Exact(Read("DifferenceSwapped")));

    /// <summary>Compiler-generated names are all `&lt;generated&gt;` to the structure, but numbered by first use here.</summary>
    [Fact]
    public void An_exact_match_keeps_which_generated_member_is_named()
    {
        Assert.Empty(Read("SameLocalFunction").Compare(Read("OtherLocalFunction")));
        Assert.False(Read("SameLocalFunction").Exact(Read("OtherLocalFunction")));
    }

    [Fact]
    public void An_exact_match_keeps_branch_direction_and_block_order()
    {
        Assert.True(Read("Pick").Exact(Read("PickAgain")));
        Assert.False(Read("Pick").Exact(Read("PickTernary")));
    }

    /// <summary>Encodings fold to one spelling (Read then renumbers locals by first use); argument numbers and branch
    /// kinds are kept.</summary>
    [Fact]
    public void Exact_matching_ignores_encodings_and_local_numbering()
    {
        Assert.Equal("ldc.i4 2", StructureReader.Folded(System.Reflection.Emit.OpCodes.Ldc_I4_2, ""));
        Assert.Equal("ldc.i4 2", StructureReader.Folded(System.Reflection.Emit.OpCodes.Ldc_I4_S, "2"));
        Assert.Equal("ldc.i4 -1", StructureReader.Folded(System.Reflection.Emit.OpCodes.Ldc_I4_M1, ""));
        Assert.Equal("ldarg 1", StructureReader.Folded(System.Reflection.Emit.OpCodes.Ldarg_1, ""));
        Assert.Equal("ldarg 1", StructureReader.Folded(System.Reflection.Emit.OpCodes.Ldarg_S, "1"));
        Assert.Equal("stloc 7", StructureReader.Folded(System.Reflection.Emit.OpCodes.Stloc_S, "7"));
        Assert.Equal("stloc 0", StructureReader.Folded(System.Reflection.Emit.OpCodes.Stloc_0, ""));
        Assert.Equal("ldloca 3", StructureReader.Folded(System.Reflection.Emit.OpCodes.Ldloca_S, "3"));
        Assert.Equal("beq", StructureReader.Folded(System.Reflection.Emit.OpCodes.Beq_S, ""));
        Assert.Equal("bne.un", StructureReader.Folded(System.Reflection.Emit.OpCodes.Bne_Un, ""));
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

    [Fact]
    public void Mutants_point_at_the_instructions_they_change()
    {
        var md = Pe.GetMetadataReader();
        var method = md.GetMethodDefinition(md.MethodDefinitions.Single(h => md.GetString(md.GetMethodDefinition(h).Name) == "Constant"
            && SigProvider.FullName(md, md.GetMethodDefinition(h).GetDeclaringType()) == typeof(StructureTests).FullName));
        var mutants = Mutants.Find(Pe, method);
        var image = File.ReadAllBytes(typeof(StructureTests).Assembly.Location);
        Assert.Contains(mutants, m => m.Description == "ldc.i4.3 -> ldc.i4.4");
        Assert.Contains(mutants, m => m.Description.StartsWith("bgt") || m.Description.StartsWith("ble") || m.Description.StartsWith("cgt"));
        foreach (var m in mutants)
            Assert.Equal(m.Original, Convert.ToHexString(image, (int)m.FileOffset, m.Original.Length / 2));
    }

    private static List<string> ConstantMutants(string name)
    {
        var md = Pe.GetMetadataReader();
        var method = md.GetMethodDefinition(md.MethodDefinitions.Single(h => md.GetString(md.GetMethodDefinition(h).Name) == name
            && SigProvider.FullName(md, md.GetMethodDefinition(h).GetDeclaringType()) == typeof(StructureTests).FullName));
        return Mutants.Find(Pe, method).Select(m => m.Description).Where(d => d.StartsWith("ldc.")).Order(StringComparer.Ordinal).ToList();
    }

    /// <summary>A constant fed straight into a framework collection's capacity constructor is not mutated; other constants are.</summary>
    [Fact]
    public void Capacity_hints_are_not_mutated() => Assert.Equal(
        new[] { "ldc.i4 2000000 -> 2000001", "ldc.i4.m1 -> ldc.i4.0", "ldc.i4.s 10 -> 11", "ldc.i4.s 11 -> 12", "ldc.i4.s 12 -> 13",
                "ldc.i4.s 120 -> 121", "ldc.i4.s 13 -> 14", "ldc.i4.s 64 -> 65" }.Order(StringComparer.Ordinal),
        ConstantMutants("Capacities"));

    /// <summary>A method that reads capacity can observe the hint, so it keeps the mutant.</summary>
    [Fact]
    public void Capacity_hints_are_mutated_where_capacity_is_read() =>
        Assert.Equal(["ldc.i4.s 100 -> 101"], ConstantMutants("CapacityRead"));

    [Fact]
    public void Quire_types_are_named_after_their_Mathcad_binding()
    {
        var mapped = Read("Ternary", n => n == typeof(StructureTests).FullName ? "Mcd.Fixture" : n);
        Assert.Contains("call Mcd.Fixture::Left`0()->System.Int32", mapped.Calls);
    }
}
