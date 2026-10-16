using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace Quire.Harness.Tests;

/// <summary>
/// MemberGen's classification and file edits on a fixture repository: a Mathcad assembly <c>mx</c> (types M.U and
/// M.T) in an install directory, and a Quire project <c>Fx</c> whose stubs mirror them, with generated files, a
/// portable PDB that places the stub methods in them, and one hand-written file declaring both partial types.
/// </summary>
public class MemberGenEmitTests : IDisposable
{
    private readonly string _dir = Directory.CreateTempSubdirectory("quire-membergen-emit-").FullName;
    private string Repo => Path.Combine(_dir, "repo");
    private string Install => Path.Combine(_dir, "install");
    private string Source(string relative) => Path.Combine(Repo, "src", "Fx", relative);

    // Generated files as MirrorGen writes them (CRLF); the stub methods' sequence points are on these lines.
    private const string GeneratedT = """
        namespace Quire.Fx
        {
            [global::Quire.Mirror.MirrorOf("M.T, mx")]
            public partial class T
            {
                [global::Quire.Mirror.MirrorStub]
                private int m_x;
                [global::Quire.Mirror.MirrorStub]
                public int GetX() => throw null;
                [global::Quire.Mirror.MirrorStub]
                public int P { get => throw null; set => throw null; }
                [global::Quire.Mirror.MirrorStub]
                public event global::System.EventHandler E;
                [global::Quire.Mirror.MirrorStub]
                public int Unlisted() => throw null;
            }
        }

        """;
    private const int GetXLine = 9, PLine = 11, UnlistedLine = 15;
    private const string GeneratedU = """
        namespace Quire.Fx
        {
            [global::Quire.Mirror.MirrorOf("M.U, mx")]
            public partial class U
            {
                [global::Quire.Mirror.MirrorStub]
                private int m_y;
                [global::Quire.Mirror.MirrorStub]
                public int GetY() => throw null;
            }
        }

        """;
    private const int GetYLine = 9;
    private const string HandWritten = """
        namespace Quire.Fx;

        public partial class U
        {
            public int Other() => 1;
        }

        public partial class T
        {
        }

        """;

    private readonly int _getY;

    public MemberGenEmitTests()
    {
        Directory.CreateDirectory(Install);
        Directory.CreateDirectory(Source("Generated/Mirror"));
        Directory.CreateDirectory(Path.Combine(Repo, "docs", "mirror", "evidence"));
        Directory.CreateDirectory(Path.Combine(Repo, "tests", "Quire.Differential.Tests"));
        File.WriteAllText(Source("Generated/Mirror/M.T.g.cs"), GeneratedT.ReplaceLineEndings("\r\n"));
        File.WriteAllText(Source("Generated/Mirror/M.U.g.cs"), GeneratedU.ReplaceLineEndings("\r\n"));
        File.WriteAllText(Source("Both.cs"), HandWritten);
        File.WriteAllText(Path.Combine(Repo, "docs", "mirror", "type-map.csv"),
            "mathcad_assembly,mathcad_type,kind,visibility,quire_type,status,phase,group\n"
            + "mx.dll,M.T,class,public,Quire.Fx.T,exists,1,x\nmx.dll,M.U,class,public,Quire.Fx.U,exists,1,x\n");
        BuildMathcad(Path.Combine(Install, "mx.dll"));
        BuildQuire(Path.Combine(_dir, "build", "Fx.dll"));
        _getY = new Artifact(Path.Combine(Install, "mx.dll")).Methods.Single(m => m.Name == "GetY").Token;
        // An older evidence record with a normalized-IL column, already holding GetY's row.
        File.WriteAllText(Path.Combine(Repo, "docs", "mirror", "evidence", "M.U.md"), $"""
            # M.U

            | Member | Token | Normalized IL | Contract | Confidence / limits | Status |
            |---|---|---|---|---|---|
            | `GetY()` | 0x{_getY:x8} | `ldarg.0 ldfld ret` | earlier | earlier | Analyzed |

            """);
    }

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    private static CustomAttributeBuilder Attribute(Type type) => new(type.GetConstructor(Type.EmptyTypes)!, []);

    private static void Ret(MethodBuilder m, Action<ILGenerator>? body = null)
    {
        var il = m.GetILGenerator();
        body?.Invoke(il);
        il.Emit(OpCodes.Ret);
    }

    private static MethodBuilder Method(TypeBuilder t, string name, Type result, Type[] parameters, bool special = false) =>
        t.DefineMethod(name, MethodAttributes.Public | (special ? MethodAttributes.SpecialName | MethodAttributes.HideBySig : 0), result, parameters);

    /// <summary>mx: M.T with a field read by GetX, an auto-property P, a field-like event E and Unlisted; M.U with GetY.</summary>
    private static void BuildMathcad(string path)
    {
        var assembly = new PersistedAssemblyBuilder(new AssemblyName("mx"), typeof(object).Assembly);
        var module = assembly.DefineDynamicModule("mx");
        var generated = Attribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute));

        var u = module.DefineType("M.U", TypeAttributes.Public);
        var y = u.DefineField("m_y", typeof(int), FieldAttributes.Private);
        Ret(Method(u, "GetY", typeof(int), []), il => { il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldfld, y); });
        u.CreateType();

        var t = module.DefineType("M.T", TypeAttributes.Public);
        var x = t.DefineField("m_x", typeof(int), FieldAttributes.Private);
        Ret(Method(t, "GetX", typeof(int), []), il => { il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldfld, x); });
        Ret(Method(t, "Unlisted", typeof(int), []), il => il.Emit(OpCodes.Ldc_I4_7));
        var backing = t.DefineField("<P>k__BackingField", typeof(int), FieldAttributes.Private);
        var get = Method(t, "get_P", typeof(int), [], special: true);
        get.SetCustomAttribute(generated);
        Ret(get, il => { il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldfld, backing); });
        var set = Method(t, "set_P", typeof(void), [typeof(int)], special: true);
        set.SetCustomAttribute(generated);
        Ret(set, il => { il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldarg_1); il.Emit(OpCodes.Stfld, backing); });
        var p = t.DefineProperty("P", PropertyAttributes.None, typeof(int), null);
        p.SetGetMethod(get);
        p.SetSetMethod(set);
        t.DefineField("E", typeof(EventHandler), FieldAttributes.Private);
        var e = t.DefineEvent("E", EventAttributes.None, typeof(EventHandler));
        foreach (string accessor in new[] { "add_E", "remove_E" })
        {
            var m = Method(t, accessor, typeof(void), [typeof(EventHandler)], special: true);
            m.SetCustomAttribute(generated);
            Ret(m);
            if (accessor == "add_E") e.SetAddOnMethod(m); else e.SetRemoveOnMethod(m);
        }
        t.CreateType();
        assembly.Save(path);
    }

    /// <summary>Fx: Quire.Fx.T and Quire.Fx.U, every member a stub, the stub methods' sequence points in the
    /// generated files; saved with its portable PDB.</summary>
    private void BuildQuire(string path)
    {
        var assembly = new PersistedAssemblyBuilder(new AssemblyName("Fx"), typeof(object).Assembly);
        var module = assembly.DefineDynamicModule("Fx");
        ConstructorBuilder AttributeType(string name, Type[] parameters)
        {
            var type = module.DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed, typeof(Attribute));
            var ctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameters);
            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(Attribute).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, [])!);
            il.Emit(OpCodes.Ret);
            type.CreateType();
            return ctor;
        }
        var mirrorOf = AttributeType("Quire.Mirror.MirrorOfAttribute", [typeof(string)]);
        var stub = new CustomAttributeBuilder(AttributeType("Quire.Mirror.MirrorStubAttribute", []), []);
        void Stub(MethodBuilder m, ISymbolDocumentWriter? document, int line)
        {
            var il = m.GetILGenerator();
            if (document != null) il.MarkSequencePoint(document, line, 9, line, 40);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Throw);
        }
        string Document(string name) => Source($"Generated/Mirror/{name}").Replace('\\', '/');


        var t = module.DefineType("Quire.Fx.T", TypeAttributes.Public);
        t.SetCustomAttribute(new CustomAttributeBuilder(mirrorOf, ["M.T, mx"]));
        t.DefineField("m_x", typeof(int), FieldAttributes.Private).SetCustomAttribute(stub);
        var docT = module.DefineDocument(Document("M.T.g.cs"));
        var getX = Method(t, "GetX", typeof(int), []);
        getX.SetCustomAttribute(stub);
        Stub(getX, docT, GetXLine);
        var unlisted = Method(t, "Unlisted", typeof(int), []);
        unlisted.SetCustomAttribute(stub);
        Stub(unlisted, docT, UnlistedLine);
        var get = Method(t, "get_P", typeof(int), [], special: true);
        Stub(get, docT, PLine);
        var set = Method(t, "set_P", typeof(void), [typeof(int)], special: true);
        Stub(set, docT, PLine);
        var p = t.DefineProperty("P", PropertyAttributes.None, typeof(int), null);
        p.SetGetMethod(get);
        p.SetSetMethod(set);
        p.SetCustomAttribute(stub);
        var e = t.DefineEvent("E", EventAttributes.None, typeof(EventHandler));
        e.SetCustomAttribute(stub);
        foreach (string accessor in new[] { "add_E", "remove_E" })
        {
            var m = Method(t, accessor, typeof(void), [typeof(EventHandler)], special: true);
            Stub(m, null, 0);
            if (accessor == "add_E") e.SetAddOnMethod(m); else e.SetRemoveOnMethod(m);
        }
        t.CreateType();

        var u = module.DefineType("Quire.Fx.U", TypeAttributes.Public);
        u.SetCustomAttribute(new CustomAttributeBuilder(mirrorOf, ["M.U, mx"]));
        u.DefineField("m_y", typeof(int), FieldAttributes.Private).SetCustomAttribute(stub);
        var docU = module.DefineDocument(Document("M.U.g.cs"));
        var getY = Method(u, "GetY", typeof(int), []);
        getY.SetCustomAttribute(stub);
        Stub(getY, docU, GetYLine);
        Ret(Method(u, "Other", typeof(int), []), il => il.Emit(OpCodes.Ldc_I4_1));
        u.CreateType();

        var metadata = assembly.GenerateMetadata(out var il, out var fieldData, out var pdb);
        var pdbBlob = new BlobBuilder();
        var pdbId = new PortablePdbBuilder(pdb, metadata.GetRowCounts(), default).Serialize(pdbBlob);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using (var stream = File.Create(Path.ChangeExtension(path, ".pdb"))) pdbBlob.WriteContentTo(stream);
        var debug = new DebugDirectoryBuilder();
        debug.AddCodeViewEntry(Path.ChangeExtension(path, ".pdb"), pdbId, portablePdbVersion: 0x0100);
        var pe = new BlobBuilder();
        new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(), new MetadataRootBuilder(metadata), il, fieldData,
            debugDirectoryBuilder: debug).Serialize(pe);
        using (var stream = File.Create(path)) pe.WriteContentTo(stream);
    }

    private (World World, Artifact Quire) Load()
    {
        string build = Path.Combine(_dir, "build", "Fx.dll");
        return (new World(Repo, Install, [build]), new Artifact(build));
    }

    /// <summary>Every Tier listed key of the fixture but <c>Unlisted</c>.</summary>
    private static HashSet<string> Listed(Artifact quire) =>
        [.. quire.Methods.Where(m => m.Owner.StartsWith("Quire.Fx.") && m.Name != "Unlisted").Select(m => $"{m.Owner}::{m.Key}")];

    private JsonElement Emit(HashSet<string>? allowed = null)
    {
        var (world, quire) = Load();
        var report = new Emitter(Repo, world, quire, "Fx", [], allowed ?? Listed(quire)).Run();
        return JsonDocument.Parse(report).RootElement;
    }

    [Fact]
    public void Stubs_are_classified_by_their_Mathcad_bodies()
    {
        var (world, quire) = Load();
        var plans = Stubs.Find(quire, world).Select(Stubs.Classify).ToDictionary(p => p.Stub.Name);
        Assert.Equal(("field", false), (plans["m_x"].Category, plans["m_x"].Mechanical));
        Assert.Equal(("field get", true), (plans["GetX"].Category, plans["GetX"].Mechanical));
        Assert.Equal(("auto-property", true), (plans["P"].Category, plans["P"].Mechanical));
        Assert.Equal(("field-like event", true), (plans["E"].Category, plans["E"].Mechanical));
        Assert.Equal(("constant", true), (plans["Unlisted"].Category, plans["Unlisted"].Mechanical));
    }

    [Fact]
    public void A_method_stub_is_identified_by_its_key_once()
    {
        var (world, quire) = Load();
        Assert.Equal("Quire.Fx.T::GetX`0()->[System.Runtime]System.Int32|False",
            Stubs.Find(quire, world).Single(s => s.Name == "GetX").Id);
    }

    [Fact]
    public void Members_move_into_the_hand_written_partial_and_their_stub_pairs_are_deleted()
    {
        var report = Emit();
        Assert.Equal("""
            using Quire.Mirror;

            namespace Quire.Fx;

            public partial class U
            {
                public int Other() => 1;

                [MirrorVerified("MUMechanicalDifferentialTests")]
                private int m_y;

                [MirrorVerified("MUMechanicalDifferentialTests")]
                public int GetY() => m_y;
            }

            public partial class T
            {
                [MirrorVerified("MTMechanicalDifferentialTests")]
                private int m_x;

                [MirrorVerified("MTMechanicalDifferentialTests")]
                public int GetX() => m_x;

                [MirrorVerified("MTMechanicalDifferentialTests")]
                public int P { get; set; }

                [MirrorVerified("MTMechanicalDifferentialTests")]
                public event EventHandler E;
            }

            """, File.ReadAllText(Source("Both.cs")));
        // Whole stub line pairs only, CRLF kept; the unlisted member stays a stub.
        Assert.Equal(string.Join("\r\n", GeneratedT.Split('\n').Where((_, i) => i is < 5 or > 12)),
            File.ReadAllText(Source("Generated/Mirror/M.T.g.cs")));
        Assert.Equal(4, report.GetProperty("members").GetArrayLength());
    }

    /// <summary>T (declared first) has its members written first, then U's go in above them: every span still names
    /// its member's lines.</summary>
    [Fact]
    public void Spans_follow_later_insertions_into_the_same_file()
    {
        var report = Emit();
        var lines = File.ReadAllText(Source("Both.cs")).Split('\n');
        var file = report.GetProperty("files").EnumerateArray().Single();
        Assert.Equal("src/Fx/Both.cs", file.GetProperty("file").GetString());
        foreach (var span in file.GetProperty("members").EnumerateArray())
        {
            string member = span.GetProperty("member").GetProperty("member").GetString()!;
            var text = lines[(span.GetProperty("first").GetInt32() - 1)..span.GetProperty("last").GetInt32()];
            Assert.StartsWith("[MirrorVerified(", text[0].Trim());
            Assert.Contains(text, l => l.Contains($" {member}"));
        }
    }

    [Fact]
    public void A_moved_field_belongs_to_the_member_that_moved_it()
    {
        var report = Emit();
        var field = report.GetProperty("files").EnumerateArray().Single().GetProperty("members").EnumerateArray()
            .Select(s => s.GetProperty("member")).Single(m => m.GetProperty("member").GetString() == "m_x");
        Assert.Equal("Quire.Fx.T::GetX`0()->[System.Runtime]System.Int32|False", field.GetProperty("owner").GetString());
    }

    [Fact]
    public void Only_listed_members_are_generated()
    {
        var report = Emit([]);
        Assert.Equal(0, report.GetProperty("members").GetArrayLength());
        Assert.Equal(HandWritten, File.ReadAllText(Source("Both.cs")));
        Assert.Equal(GeneratedT.ReplaceLineEndings("\r\n"), File.ReadAllText(Source("Generated/Mirror/M.T.g.cs")));
    }

    [Fact]
    public void Tests_and_evidence_are_written_per_Mathcad_type()
    {
        Emit();
        string tests = File.ReadAllText(Path.Combine(Repo, "tests", "Quire.Differential.Tests", "MTMechanicalDifferentialTests.cs"));
        Assert.Contains("public class MTMechanicalDifferentialTests", tests);
        Assert.Contains("Mechanical.Compare(typeof(global::Quire.Fx.T), 0x", tests);
        Assert.Contains("public void E_add_", tests);
        Assert.DoesNotContain("Unlisted", tests);
        string evidence = File.ReadAllText(Path.Combine(Repo, "docs", "mirror", "evidence", "M.T.md"));
        Assert.Contains("| `GetX()` | 0x", evidence);
        Assert.Contains("| `P get` | 0x", evidence);
    }

    /// <summary>A row for the same token is replaced, but an older record's normalized IL stays.</summary>
    [Fact]
    public void A_replaced_evidence_row_keeps_its_normalized_IL()
    {
        Emit();
        var row = File.ReadAllLines(Path.Combine(Repo, "docs", "mirror", "evidence", "M.U.md")).Single(l => l.StartsWith("| `GetY()`"));
        Assert.StartsWith($"| `GetY()` | 0x{_getY:x8} | `ldarg.0 ldfld ret` | MemberGen: `m_y` |", row);
        Assert.EndsWith("| Verified |", row);
    }
}
