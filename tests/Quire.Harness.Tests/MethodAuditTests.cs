using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Quire.Harness.Tests;

/// <summary>MethodAudit run in-process over emitted assemblies: a stand-in Mathcad original in an install directory and
/// the Quire assembly that mirrors it, bound through a one-row type map.</summary>
public class MethodAuditTests
{
    /// <summary>C++/CLI overloads that differ only in a custom modifier (<c>void F(int)</c>, <c>void F(long)</c> =
    /// <c>int32 modopt(IsLong)</c>) share a key. Each row still gets its own id; a member whose key is its own keeps
    /// the plain id.</summary>
    [Fact]
    public void Type_members_that_differ_only_in_custom_modifiers_get_distinct_ids()
    {
        string work = Directory.CreateTempSubdirectory("quire-method-audit-").FullName;
        try
        {
            string repo = Path.Combine(work, "repo"), install = Path.Combine(work, "install"), assemblies = Path.Combine(work, "assemblies");
            Directory.CreateDirectory(Path.Combine(repo, "docs", "mirror"));
            Directory.CreateDirectory(Path.Combine(repo, "src", "Quire.P"));
            File.WriteAllText(Path.Combine(repo, "docs", "mirror", "type-map.csv"),
                "mathcad_assembly,mathcad_type,kind,visibility,quire_type,status,phase,group\nOrig.dll,N.T,class,public,Quire.N.T,missing,1,test\n");
            File.WriteAllText(Path.Combine(repo, "docs", "mirror", "generated-assemblies.json"), "{}");
            Emit("Orig", "N.T", null, install);
            Emit("Quire.P", "Quire.N.T", "N.T, Orig", assemblies);
            string output = Path.Combine(work, "inventory.json");

            var code = Assembly.Load("Quire.Tools.MethodAudit").EntryPoint!.Invoke(null,
                [new[] { "--repo", repo, "--assemblies", assemblies, "--install", install, "--output", output, "--project", "Quire.P" }]);

            Assert.Equal(0, code);
            var rows = JsonDocument.Parse(File.ReadAllText(output)).RootElement.GetProperty("methods").EnumerateArray()
                .Where(r => r.GetProperty("type").GetString() == "Quire.N.T").ToList();
            string Id(JsonElement row) => row.GetProperty("id").GetString()!;
            string Declared(JsonElement row) => row.GetProperty("declared_signature").GetString()!;
            Assert.Equal(rows.Count, rows.Select(Id).Distinct().Count());
            var f = rows.Where(r => Declared(r).StartsWith("F`")).ToList();
            Assert.Equal(2, f.Count);
            Assert.Equal(Declared(f[0]), Declared(f[1]));
            Assert.Equal(["Quire.P::Quire.N.T::" + Declared(f[0]) + " [;]",
                          "Quire.P::Quire.N.T::" + Declared(f[0]) + " [;modopt(System.Runtime.CompilerServices.IsLong)]"],
                f.Select(Id).Order(StringComparer.Ordinal));
            // Each overload still pairs with the original of its own modifiers.
            Assert.All(f, r => Assert.True(r.GetProperty("exact_match").GetBoolean()));
            var g = rows.Single(r => Declared(r).StartsWith("G`"));
            Assert.Equal("Quire.P::Quire.N.T::" + Declared(g), Id(g));
        }
        finally { Directory.Delete(work, true); }
    }

    /// <summary>An assembly declaring <paramref name="type"/> with <c>F(int)</c>, <c>F(int modopt(IsLong))</c> and
    /// <c>G(int)</c>, bound to <paramref name="mirrorOf"/> when given.</summary>
    private static void Emit(string name, string type, string? mirrorOf, string directory)
    {
        var assembly = new PersistedAssemblyBuilder(new AssemblyName(name), typeof(object).Assembly);
        var module = assembly.DefineDynamicModule(name);
        var t = module.DefineType(type, TypeAttributes.Public);
        if (mirrorOf != null)
        {
            var attribute = module.DefineType("Quire.Mirror.MirrorOfAttribute", TypeAttributes.Public | TypeAttributes.Sealed, typeof(Attribute));
            var constructor = attribute.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, [typeof(string)]);
            var body = constructor.GetILGenerator();
            body.Emit(OpCodes.Ldarg_0);
            body.Emit(OpCodes.Call, typeof(Attribute).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!);
            body.Emit(OpCodes.Ret);
            attribute.CreateType();
            t.SetCustomAttribute(new CustomAttributeBuilder(constructor, [mirrorOf]));
        }
        foreach (Type[] modifiers in new[] { Type.EmptyTypes, [typeof(IsLong)] })
            t.DefineMethod("F", MethodAttributes.Public, CallingConventions.Standard, typeof(void), null, null,
                [typeof(int)], null, [modifiers]).GetILGenerator().Emit(OpCodes.Ret);
        t.DefineMethod("G", MethodAttributes.Public, typeof(void), [typeof(int)]).GetILGenerator().Emit(OpCodes.Ret);
        t.CreateType();
        Directory.CreateDirectory(directory);
        assembly.Save(Path.Combine(directory, name + ".dll"));
    }
}
