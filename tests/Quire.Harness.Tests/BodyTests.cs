using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Quire.Tools.MirrorGen;

namespace Quire.Harness.Tests;

/// <summary>What MethodAudit records of a body to identify changed methods: its hash.</summary>
public class BodyTests
{
    private static int Data(int i) => new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }[i];
    private static int OtherData(int i) => new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18 }[i];
    private static int Constant() => 1;
    private static int Constant2() => 1;
    private static string Where([System.Runtime.CompilerServices.CallerFilePath] string file = "") => file;
    private static string Here() => Where();
    private static string SpelledUnix() => Where("<repo>/tests/Quire.Harness.Tests/BodyTests.cs");
    private static string SpelledWindows() => Where("<repo>\\tests\\Quire.Harness.Tests\\BodyTests.cs");

    private static readonly PEReader Pe = new(File.OpenRead(typeof(BodyTests).Assembly.Location));
    private static readonly MetadataReader Md = Pe.GetMetadataReader();
    private static readonly StructureReader Reader = new(Md, Pe, new SigProvider("Quire.Harness.Tests"), _ => null);

    private static MethodDefinitionHandle Find(Type type, string name) => Md.MethodDefinitions.Single(h =>
        Md.GetString(Md.GetMethodDefinition(h).Name) == name
        && SigProvider.FullName(Md, Md.GetMethodDefinition(h).GetDeclaringType()) == type.FullName);

    private static StructureReader.BodyFacts Body(string name) => Reader.Body(Find(typeof(BodyTests), name));

    [Fact]
    public void The_hash_follows_the_body_not_the_token_or_name()
    {
        Assert.Equal(Body("Constant").Hash, Body("Constant2").Hash);
        Assert.NotEqual(Body("Data").Hash, Body("OtherData").Hash);
    }

    [Fact]
    public void The_hash_leaves_out_the_checkout_path_that_CallerFilePath_compiles_in()
    {
        // The gate builds each candidate in a new clone; a stub's MirrorStub.NotImplemented() embeds its source path.
        string root = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Where())!, "..", ".."));
        var rooted = new StructureReader(Md, Pe, new SigProvider("Quire.Harness.Tests"), _ => null, root: root);
        var elsewhere = new StructureReader(Md, Pe, new SigProvider("Quire.Harness.Tests"), _ => null,
            root: Path.Combine(root, "elsewhere"));
        MethodDefinitionHandle Method(string name) => Find(typeof(BodyTests), name);
        string spelled = Path.DirectorySeparatorChar == '/' ? "SpelledUnix" : "SpelledWindows";
        Assert.Equal(rooted.Body(Method(spelled)).Hash, rooted.Body(Method("Here")).Hash);
        Assert.NotEqual(elsewhere.Body(Method(spelled)).Hash, elsewhere.Body(Method("Here")).Hash);
        Assert.NotEqual(rooted.Body(Method("SpelledUnix")).Hash, rooted.Body(Method("SpelledWindows")).Hash);
    }

    [Fact]
    public void Only_a_whole_checkout_path_prefix_is_replaced()
    {
        string root = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Where())!, "..", ".."));
        var slashed = new StructureReader(Md, Pe, new SigProvider("Quire.Harness.Tests"), _ => null,
            root: root + Path.DirectorySeparatorChar);
        string spelled = Path.DirectorySeparatorChar == '/' ? "SpelledUnix" : "SpelledWindows";
        Assert.Equal(slashed.Body(Find(typeof(BodyTests), spelled)).Hash, slashed.Body(Find(typeof(BodyTests), "Here")).Hash);
        Assert.Equal("<repo>/src/a.cs", StructureReader.Unrooted("/w/repo/", "/w/repo/src/a.cs"));
        Assert.Equal("<repo>", StructureReader.Unrooted("/w/repo", "/w/repo"));
        Assert.Equal("/w/repo2/src/a.cs", StructureReader.Unrooted("/w/repo", "/w/repo2/src/a.cs"));
        Assert.Equal(@"<repo>\src\a.cs", StructureReader.Unrooted(@"C:\w\repo", @"C:\w\repo\src\a.cs"));
        Assert.Equal("/w/x.cs", StructureReader.Unrooted("/", "/w/x.cs"));
    }
}
