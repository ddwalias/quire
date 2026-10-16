using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;

namespace Quire.Harness.Tests;

/// <summary>MethodAudit's --structure-assemblies: structure is compared on an optimized build, like Mathcad's.</summary>
public class OptimizedBuildTests : IDisposable
{
    private readonly string _dir = Directory.CreateTempSubdirectory("quire-optimized-").FullName;

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    /// <summary>An assembly with type <paramref name="type"/> whose static int F(int) has the given body, and an
    /// <paramref name="extra"/> method; <paramref name="helpers"/> compiler-generated methods come first, so they
    /// shift the tokens of the rest.</summary>
    private string Build(string folder, string name, string type, Action<ILGenerator> body, string? mirrorOf = null,
        string? extra = null, int helpers = 0, Action<TypeBuilder>? more = null)
    {
        var assembly = new PersistedAssemblyBuilder(new AssemblyName(name), typeof(object).Assembly);
        var module = assembly.DefineDynamicModule(name);
        if (helpers > 0)
        {
            var generated = module.DefineType("<PrivateImplementationDetails>", TypeAttributes.Sealed | TypeAttributes.Abstract);
            for (int i = 0; i < helpers; i++)
                generated.DefineMethod($"<H>{i}", MethodAttributes.Assembly | MethodAttributes.Static).GetILGenerator().Emit(OpCodes.Ret);
            generated.CreateType();
        }
        var t = module.DefineType(type, TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed);
        if (mirrorOf != null)
        {
            var attribute = module.DefineType("Quire.Mirror.MirrorOfAttribute", TypeAttributes.Public | TypeAttributes.Sealed, typeof(Attribute));
            var ctor = attribute.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, [typeof(string)]);
            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(Attribute).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, [])!);
            il.Emit(OpCodes.Ret);
            attribute.CreateType();
            t.SetCustomAttribute(new CustomAttributeBuilder(ctor, [mirrorOf]));
        }
        body(t.DefineMethod("F", MethodAttributes.Public | MethodAttributes.Static, typeof(int), [typeof(int)]).GetILGenerator());
        if (extra != null)
            t.DefineMethod(extra, MethodAttributes.Public | MethodAttributes.Static).GetILGenerator().Emit(OpCodes.Ret);
        more?.Invoke(t);
        t.CreateType();
        string path = Path.Combine(_dir, folder, name + ".dll");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        assembly.Save(path);
        return path;
    }

    // `x > 2 ? 1 : 0` as Roslyn compiles it without and with optimization; Mathcad's assemblies are optimized.
    private static void Debug(ILGenerator il)
    {
        Label one = il.DefineLabel(), end = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4_2);
        il.Emit(OpCodes.Bgt_S, one);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Br_S, end);
        il.MarkLabel(one);
        il.Emit(OpCodes.Ldc_I4_1);
        il.MarkLabel(end);
        il.Emit(OpCodes.Ret);
    }

    private static void Optimized(ILGenerator il)
    {
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4_2);
        il.Emit(OpCodes.Cgt);
        il.Emit(OpCodes.Ret);
    }

    // `if (x > 2) return x; return 2;` and `x <= 2 ? 2 : x`: the same structure, branches laid out the other way.
    private static void IfReturn(ILGenerator il) => Pick(il, OpCodes.Ble_S, OpCodes.Ldarg_0, OpCodes.Ldc_I4_2);
    private static void Ternary(ILGenerator il) => Pick(il, OpCodes.Bgt_S, OpCodes.Ldc_I4_2, OpCodes.Ldarg_0);

    private static void Pick(ILGenerator il, OpCode branch, OpCode first, OpCode second)
    {
        Label other = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4_2);
        il.Emit(branch, other);
        il.Emit(first);
        il.Emit(OpCodes.Ret);
        il.MarkLabel(other);
        il.Emit(second);
        il.Emit(OpCodes.Ret);
    }

    /// <summary>IfReturn with long encodings: <c>ble</c>, <c>ldarg 0</c>, <c>ldc.i4 2</c>.</summary>
    private static void IfReturnLong(ILGenerator il)
    {
        Label other = il.DefineLabel();
        il.Emit(OpCodes.Ldarg, (short)0);
        il.Emit(OpCodes.Ldc_I4, 2);
        il.Emit(OpCodes.Ble, other);
        il.Emit(OpCodes.Ldarg, (short)0);
        il.Emit(OpCodes.Ret);
        il.MarkLabel(other);
        il.Emit(OpCodes.Ldc_I4, 2);
        il.Emit(OpCodes.Ret);
    }

    /// <summary>`x > 2` through local <paramref name="slot"/> of two.</summary>
    private static void ThroughLocal(ILGenerator il, short slot)
    {
        il.DeclareLocal(typeof(int));
        il.DeclareLocal(typeof(int));
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Stloc, slot);
        il.Emit(OpCodes.Ldloc, slot);
        il.Emit(OpCodes.Ldc_I4_2);
        il.Emit(OpCodes.Cgt);
        il.Emit(OpCodes.Ret);
    }

    /// <summary>Runs MethodAudit on a Debug build of Fix.T, bound to the original Orig.T (body
    /// <paramref name="original"/>, default <see cref="Optimized"/>), with an optimized build (none: an empty folder)
    /// when <paramref name="release"/> is given, and returns its exit code and inventory.</summary>
    private (int Exit, JsonElement Inventory) Audit(Action<ILGenerator>? release, string? extra = null, int helpers = 0,
        bool none = false, Action<TypeBuilder>? more = null, Action<ILGenerator>? original = null,
        Action<TypeBuilder>? moreOriginal = null, string bindings = "")
    {
        Directory.CreateDirectory(Path.Combine(_dir, "repo/docs/mirror"));
        File.WriteAllText(Path.Combine(_dir, "repo/docs/mirror/type-map.csv"),
            "mathcad_assembly,mathcad_type,kind,visibility,quire_type,status,phase,group\nOrig.dll,Orig.T,class,public,Fix.T,exists,1,managed\n" + bindings);
        Build("install", "Orig", "Orig.T", original ?? Optimized, more: moreOriginal);
        Build("debug", "Fix", "Fix.T", Debug, "Orig.T, Orig", helpers: helpers, more: more);
        string output = Path.Combine(_dir, "inventory.json");
        List<string> args = ["--repo", Path.Combine(_dir, "repo"), "--assemblies", Path.Combine(_dir, "debug"),
            "--install", Path.Combine(_dir, "install"), "--output", output, "--project", "Fix"];
        if (release != null)
        {
            Directory.CreateDirectory(Path.Combine(_dir, "release"));
            if (!none) Build("release", "Fix", "Fix.T", release, "Orig.T, Orig", extra, more: more);
            args.AddRange(["--structure-assemblies", Path.Combine(_dir, "release")]);
        }
        var tool = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Quire.Tools.MethodAudit.dll"));
        int exit = (int)tool.EntryPoint!.Invoke(null, [args.ToArray()])!;
        return (exit, JsonDocument.Parse(File.ReadAllText(output)).RootElement);
    }

    private static JsonElement F(JsonElement inventory) =>
        inventory.GetProperty("methods").EnumerateArray().Single(m => m.GetProperty("type").GetString() == "Fix.T"
            && m.GetProperty("signature").GetString()!.StartsWith("F`"));

    [Fact]
    public void Structure_comes_from_the_optimized_build_and_everything_else_from_the_Debug_build()
    {
        var (exit, debugOnly) = Audit(null);
        Assert.Equal(0, exit);
        Assert.False(F(debugOnly).GetProperty("structure_matches").GetBoolean());
        var (_, inventory) = Audit(Optimized);
        Assert.True(F(inventory).GetProperty("structure_matches").GetBoolean());
        // The body hash, and so mutant selection, still follow the Debug build.
        Assert.Equal(F(debugOnly).GetProperty("body_sha256").GetString(), F(inventory).GetProperty("body_sha256").GetString());
    }

    [Fact]
    public void An_exact_match_needs_the_same_instructions_in_the_same_order()
    {
        var (_, debugOnly) = Audit(null);
        Assert.False(F(debugOnly).GetProperty("exact_match").GetBoolean());
        Assert.True(F(Audit(Optimized).Inventory).GetProperty("exact_match").GetBoolean());
        // Short and long encodings of the same instructions match exactly, and so do differently numbered locals.
        Assert.True(F(Audit(IfReturnLong, original: IfReturn).Inventory).GetProperty("exact_match").GetBoolean());
        Assert.True(F(Audit(il => ThroughLocal(il, 0), original: il => ThroughLocal(il, 1)).Inventory)
            .GetProperty("exact_match").GetBoolean());
        // A ternary for if/return keeps the structure but not the branch direction and block order.
        var ternary = F(Audit(Ternary, original: IfReturn).Inventory);
        Assert.True(ternary.GetProperty("structure_matches").GetBoolean());
        Assert.False(ternary.GetProperty("exact_match").GetBoolean());
    }

    [Fact]
    public void Compiler_generated_methods_may_differ_and_renumber_the_rest()
    {
        var (exit, inventory) = Audit(Optimized, helpers: 3);
        Assert.Equal(0, exit);
        Assert.True(F(inventory).GetProperty("structure_matches").GetBoolean());
    }

    [Fact]
    public void An_optimized_build_declaring_other_methods_is_refused_naming_the_first()
    {
        var (exit, inventory) = Audit(Optimized, extra: "G");
        Assert.Equal(1, exit);
        string error = Assert.Single(inventory.GetProperty("errors").EnumerateArray()).GetString()!;
        Assert.Contains("only in Debug none, only optimized Fix.T::G`0()", error);
        Assert.Empty(inventory.GetProperty("methods").EnumerateArray());
    }

    [Fact]
    public void A_missing_optimized_build_is_refused()
    {
        var (exit, inventory) = Audit(Optimized, none: true);
        Assert.Equal(1, exit);
        Assert.Contains("Fix: cannot inspect required assembly", Assert.Single(inventory.GetProperty("errors").EnumerateArray()).GetString());
    }

    /// <summary>The optimized build drops a [Conditional] method's calls, and calls to Debug and Contract.</summary>
    [Fact]
    public void Code_the_optimized_build_compiles_away_is_refused()
    {
        var conditional = Audit(Optimized, more: t =>
        {
            var log = t.DefineMethod("Log", MethodAttributes.Public | MethodAttributes.Static);
            log.SetCustomAttribute(new CustomAttributeBuilder(typeof(System.Diagnostics.ConditionalAttribute).GetConstructor([typeof(string)])!, ["DEBUG"]));
            log.GetILGenerator().Emit(OpCodes.Ret);
        });
        Assert.Equal(1, conditional.Exit);
        Assert.Contains("Fix.T::Log`0()", Assert.Single(conditional.Inventory.GetProperty("errors").EnumerateArray()).GetString());
        var assert = Audit(Optimized, more: t =>
        {
            var il = t.DefineMethod("Check", MethodAttributes.Public | MethodAttributes.Static).GetILGenerator();
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Call, typeof(System.Diagnostics.Debug).GetMethod("Assert", [typeof(bool)])!);
            il.Emit(OpCodes.Ret);
        });
        Assert.Equal(1, assert.Exit);
        Assert.Contains("Fix.T::Check`0()->[System.Runtime]System.Void|True calls System.Diagnostics.Debug",
            Assert.Single(assert.Inventory.GetProperty("errors").EnumerateArray()).GetString());
    }

    /// <summary>A signature naming a mirrored type of a project not audited (Fix.Dep in FixDep, bound to Orig.Dep in
    /// OrigDep) still finds its counterpart, through the reviewed type map.</summary>
    [Fact]
    public void Types_of_projects_not_audited_map_through_the_type_map()
    {
        Build("install", "OrigDep", "Orig.Dep", Optimized);
        Build("debug", "FixDep", "Fix.Dep", Optimized, "Orig.Dep, OrigDep");
        var dep = new System.Runtime.Loader.AssemblyLoadContext("dependencies");
        var quire = dep.LoadFromAssemblyPath(Path.Combine(_dir, "debug", "FixDep.dll")).GetType("Fix.Dep")!;
        var mathcad = dep.LoadFromAssemblyPath(Path.Combine(_dir, "install", "OrigDep.dll")).GetType("Orig.Dep")!;
        Action<TypeBuilder> G(Type parameter) => t =>
            t.DefineMethod("G", MethodAttributes.Public | MethodAttributes.Static, typeof(void), [parameter]).GetILGenerator().Emit(OpCodes.Ret);
        var (exit, inventory) = Audit(Optimized, more: G(quire), moreOriginal: G(mathcad),
            bindings: "OrigDep.dll,Orig.Dep,class,public,Fix.Dep,exists,1,managed\n");
        Assert.Equal(0, exit);
        var g = inventory.GetProperty("methods").EnumerateArray().Single(m => m.GetProperty("signature").GetString()!.StartsWith("G`"));
        Assert.True(g.GetProperty("exact_match").GetBoolean());
    }
}
