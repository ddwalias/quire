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
    private static readonly PEReader Pe = new(File.OpenRead(typeof(StructureTests).Assembly.Location));
    private static Structure Read(string name, Func<string, string>? typeName = null)
    {
        var md = Pe.GetMetadataReader();
        var handle = md.MethodDefinitions.Single(h => md.GetString(md.GetMethodDefinition(h).Name) == name
            && SigProvider.FullName(md, md.GetMethodDefinition(h).GetDeclaringType()) == typeof(StructureTests).FullName);
        return new StructureReader(md, Pe, new SigProvider("Quire.Harness.Tests"), typeName ?? (n => n), _ => null).Read(handle, true);
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
        // Release optimization gives these two source forms identical IL.
        Assert.True(Read("Pick").Exact(Read("PickTernary")));
        Assert.False(Read("IfElse").Exact(Read("Ternary")));
    }

    /// <summary>Diagnostic opcode expansion preserves operands. Release matching uses the original encodings.</summary>
    [Fact]
    public void Diagnostic_opcode_expansion_preserves_operands()
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

    /// <summary>The reader skips a switch's count and all its targets, so the instructions after it still count. The
    /// target is 0x24 bytes on, an unassigned opcode: a reader four bytes short decodes it and fails.</summary>
    [Fact]
    public void Test_type_references_are_read_past_a_switch()
    {
        var assembly = new System.Reflection.Emit.PersistedAssemblyBuilder(new System.Reflection.AssemblyName("Switch"), typeof(object).Assembly);
        var module = assembly.DefineDynamicModule("Switch");
        var referenced = module.DefineType("B", System.Reflection.TypeAttributes.Public);
        referenced.CreateType();
        var t = module.DefineType("A", System.Reflection.TypeAttributes.Public);
        var il = t.DefineMethod("F", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static, typeof(void), [typeof(int)]).GetILGenerator();
        var target = il.DefineLabel();
        il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
        il.Emit(System.Reflection.Emit.OpCodes.Switch, [target]);
        il.Emit(System.Reflection.Emit.OpCodes.Ldtoken, referenced);                                              // 5 bytes
        il.Emit(System.Reflection.Emit.OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))!);    // 5
        il.Emit(System.Reflection.Emit.OpCodes.Pop);                                                              // 1
        for (int i = 11; i < 0x24; i++) il.Emit(System.Reflection.Emit.OpCodes.Nop);
        il.MarkLabel(target);
        il.Emit(System.Reflection.Emit.OpCodes.Ret);
        t.CreateType();
        var image = new MemoryStream();
        assembly.Save(image);
        image.Position = 0;
        using var pe = new PEReader(image);
        var md = pe.GetMetadataReader();
        var a = md.TypeDefinitions.Single(h => SigProvider.FullName(md, h) == "A");
        Assert.Equal(["B"], TestTypes.References(md, pe, a).Select(h => TestTypes.Name(md, h)));
    }

    [Fact]
    public void Quire_types_are_named_after_their_Mathcad_binding()
    {
        var mapped = Read("Ternary", n => n == typeof(StructureTests).FullName ? "Mcd.Fixture" : n);
        Assert.Contains("call Mcd.Fixture::Left`0()->System.Int32", mapped.Calls);
    }
}
