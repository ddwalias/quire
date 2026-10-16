using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Quire.Tools.MirrorGen;

namespace Quire.Harness.Tests;

/// <summary>MemberGen's decompiler and renderer on bodies emitted as Roslyn compiles them with optimization.</summary>
public class MemberGenTests : IDisposable
{
    private readonly string _dir = Directory.CreateTempSubdirectory("quire-membergen-").FullName;
    private readonly Artifact _artifact;
    private readonly World _world;

    public MemberGenTests()
    {
        var assembly = new PersistedAssemblyBuilder(new AssemblyName("Fx"), typeof(object).Assembly);
        var module = assembly.DefineDynamicModule("Fx");
        var b = module.DefineType("Fx.B", TypeAttributes.Public);
        var n = b.DefineMethod("N", MethodAttributes.Public, typeof(int), []);
        Emit(n.GetILGenerator(), il => il.Emit(OpCodes.Ldc_I4_1));
        var v = b.DefineMethod("V", MethodAttributes.Public | MethodAttributes.Virtual, typeof(int), []);
        Emit(v.GetILGenerator(), il => il.Emit(OpCodes.Ldc_I4_2));
        b.CreateType();
        var t = module.DefineType("Fx.T", TypeAttributes.Public, b);
        var x = t.DefineField("_x", typeof(int), FieldAttributes.Private);
        var ctor = t.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, [typeof(int)]);
        Emit(ctor.GetILGenerator(), il =>
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(object).GetConstructor([])!);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, x);
        });
        Method(t, "GetX", typeof(int), [], il => { il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldfld, x); }, instance: true);
        Method(t, "IsText", typeof(bool), [typeof(object)], il =>
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, typeof(string));
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Cgt_Un);
        });
        Method(t, "Differ", typeof(bool), [typeof(int), typeof(int)], il =>
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ceq);
        });
        Method(t, "Same", typeof(int), [typeof(int), typeof(int)], il =>
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ceq);
        });
        Method(t, "Shift", typeof(long), [typeof(long), typeof(int)], il =>
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4_S, (sbyte)63);
            il.Emit(OpCodes.And);
            il.Emit(OpCodes.Shl);
        });
        Method(t, "Big", typeof(long), [], il => il.Emit(OpCodes.Ldc_I8, 5L));
        // Inherited methods called non-virtually on this: only the virtual one needs `base.`.
        Method(t, "CallN", typeof(int), [], il => { il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Call, n); }, instance: true);
        Method(t, "CallV", typeof(int), [], il => { il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Call, v); }, instance: true);
        Method(t, "Kind", typeof(Type), [], il =>
        {
            il.Emit(OpCodes.Ldtoken, typeof(string));
            il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))!);
        });
        Method(t, "Branchy", typeof(int), [typeof(int)], il =>
        {
            var one = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Brtrue_S, one);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(one);
            il.Emit(OpCodes.Ldc_I4_1);
        });
        Method(t, "Local", typeof(int), [typeof(int)], il =>
        {
            il.DeclareLocal(typeof(int));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
        });
        t.CreateType();
        string path = Path.Combine(_dir, "Fx.dll");
        assembly.Save(path);

        string repo = Directory.CreateDirectory(Path.Combine(_dir, "repo", "docs", "mirror")).Parent!.Parent!.FullName;
        File.WriteAllText(Path.Combine(repo, "docs", "mirror", "type-map.csv"), "assembly,mathcad,a,b,quire,status\n");
        _artifact = new Artifact(path);
        _world = new World(repo, _dir, [path]);
    }

    public void Dispose()
    {
        _artifact.Pe.Dispose();
        Directory.Delete(_dir, recursive: true);
    }

    private static void Emit(ILGenerator il, Action<ILGenerator> body)
    {
        body(il);
        il.Emit(OpCodes.Ret);
    }

    private static void Method(TypeBuilder t, string name, Type result, Type[] parameters, Action<ILGenerator> body, bool instance = false) =>
        Emit(t.DefineMethod(name, MethodAttributes.Public | (instance ? 0 : MethodAttributes.Static), result, parameters).GetILGenerator(), body);

    private Body Decompile(string name) => Decompiler.Decompile(_artifact,
        MetadataTokens.MethodDefinitionHandle(_artifact.Methods.Single(m => m.Owner == "Fx.T" && m.Name == name).Token));

    private Renderer.Text Render(string name, params string[] parameters) =>
        new Renderer(_world, new Names("Fx", _world), _world.QuireTypes["Fx.T"], parameters).Render(Decompile(name), name == ".ctor");

    [Theory]
    [InlineData("GetX", "_x")]
    [InlineData("IsText", "o is string", "o")]
    [InlineData("Differ", "a != b", "a", "b")]
    [InlineData("Same", "a == b ? 1 : 0", "a", "b")]
    [InlineData("Shift", "v << n", "v", "n")]
    [InlineData("Big", "5L")]
    [InlineData("Kind", "typeof(string)")]
    [InlineData("CallN", "N()")]
    [InlineData("CallV", "base.V()")]
    public void Lowered_forms_render_as_their_source(string method, string expression, params string[] parameters) =>
        Assert.Equal(expression, Render(method, parameters).Expression);

    [Fact]
    public void A_constructor_that_stores_its_argument_needs_no_initializer()
    {
        var text = Render(".ctor", "x");
        Assert.Equal("ctor: store arguments", Decompile(".ctor").Kind(constructor: true));
        Assert.Null(text.Initializer);
        Assert.Null(text.Expression);
        Assert.Equal(["_x = x;"], text.Statements);
    }

    [Theory]
    [InlineData("GetX", "field get")]
    [InlineData("Big", "constant")]
    [InlineData("Kind", "cast or conversion")]
    [InlineData("Shift", "one expression")]
    public void Bodies_are_classified_by_shape(string method, string kind) =>
        Assert.Equal(kind, Decompile(method).Kind(constructor: false));

    [Theory]
    [InlineData("Branchy", "branches")]
    [InlineData("Local", "locals")]
    public void Control_flow_and_locals_are_not_mechanical(string method, string category) =>
        Assert.Equal(category, Assert.Throws<NotMechanical>(() => Decompile(method)).Message);

    [Fact]
    public void Short_and_macro_forms_are_widened()
    {
        var body = _artifact.Pe.GetMethodBody(_artifact.Metadata.GetMethodDefinition(
            MetadataTokens.MethodDefinitionHandle(_artifact.Methods.Single(m => m.Name == "Shift").Token)).RelativeVirtualAddress);
        Assert.Equal([("ldarg", (object?)0), ("ldarg", 1), ("ldc.i4", 63), ("and", null), ("shl", null), ("ret", null)],
            Il.Read(body).Select(i => (i.Op.Name!, i.Operand)));
    }

    [Fact]
    public void Nested_negation_does_not_become_a_decrement()
    {
        var x = new Arg(0, Decompiler.Int32);
        var renderer = new Renderer(_world, new Names("Fx", _world), _world.QuireTypes["Fx.T"], ["x"]);
        Assert.Equal("-(-x)", renderer.Expression(new Unary("-", new Unary("-", x)), Decompiler.Int32));
        Assert.Equal("-x", renderer.Expression(new Unary("-", x), Decompiler.Int32));
    }

    private string Named(Names names, string type) => names.Type(new NamedSig("Lib", type));

    /// <summary>A simple name only where no nearer or imported namespace declares the same one.</summary>
    [Fact]
    public void Type_names_are_simple_only_where_unambiguous()
    {
        _world.KnownTypes.UnionWith(["X.Y.Widget", "A.Gadget", "Q.Gadget", "System.IO.Gadget", "P.Thing", "R.Thing", "R.Part"]);
        var names = new Names("A.B", _world);
        Assert.Equal("Widget", Named(names, "X.Y.Widget"));
        Assert.Contains("X.Y", names.Usings);
        // A.Gadget is nearer than an import; System.IO, imported implicitly, declares Gadget too.
        Assert.Equal("global::Q.Gadget", Named(names, "Q.Gadget"));
        // Thing is taken by P; importing R for Part would make it ambiguous.
        Assert.Equal("Thing", Named(names, "P.Thing"));
        Assert.Equal("global::R.Part", Named(names, "R.Part"));
        Assert.DoesNotContain("R", names.Usings);
    }

    /// <summary>An evidence row's contract is the member's body, not its declaration.</summary>
    [Theory]
    [InlineData("public int GetX() => m_x;", "`m_x`")]
    [InlineData("public static void conv_void(int x) { }", "`{ }`")]
    [InlineData("public T(int x) : base(x) { }", "`: base(x) { }`")]
    [InlineData("public int P { get; set; }", "`public int P { get; set; }`")]
    public void Evidence_contracts_show_the_body(string line, string contract) => Assert.Equal(contract, Emitter.Contract([line]));

    [Fact]
    public void Names_use_keyword_aliases_and_escape_reserved_words()
    {
        Assert.Equal("int", Names.Keyword("System.Int32"));
        Assert.Equal("@class", Names.Identifier("class"));
        Assert.Equal("value", Names.Identifier("value"));
    }
}
