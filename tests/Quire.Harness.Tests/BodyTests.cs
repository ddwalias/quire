using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Quire.Tools.MirrorGen;

namespace Quire.Harness.Tests;

/// <summary>What MethodAudit records of a body to select mutants: its hash, calls, virtual call slots and fields.</summary>
public class BodyTests
{
    public abstract class SlotBase<T> { public abstract int M(T x); }
    public sealed class SlotDerived : SlotBase<int> { public override int M(int x) => x; }
    public class GenericDerived<T> : SlotBase<T> { public override int M(T x) => 0; }
    public interface ISlot<T> { int Run(T x); }
    public sealed class SlotImpl : ISlot<string> { int ISlot<string>.Run(string x) => 1; }
    public sealed class Holder<T> { public T? Value; }

    private static int CallBase(SlotBase<int> b) => b.M(1);
    private static int CallInterface(ISlot<string> s) => s.Run("");
    private static int Data(int i) => new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }[i];
    private static int OtherData(int i) => new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18 }[i];
    private static void Write(Holder<int> h) => h.Value = 1;
    private static int ReadValue(Holder<int> h) => h.Value;
    private static readonly int[] Table = new int[3];
    private static string Label = "";
    private static void Fill() => Table[0] = Label.Length;
    private static IEnumerable<int> Items() { yield return 1; }
    private static async Task<int> Later() { await Task.Yield(); return 1; }
    private static int Constant() => 1;
    private static int Constant2() => 1;

    private static readonly PEReader Pe = new(File.OpenRead(typeof(BodyTests).Assembly.Location));
    private static readonly MetadataReader Md = Pe.GetMetadataReader();
    private static readonly StructureReader Reader = new(Md, Pe, new SigProvider("Quire.Harness.Tests"), n => n, _ => null, exact: true);

    private static MethodDefinitionHandle Find(Type type, string name) => Md.MethodDefinitions.Single(h =>
        Md.GetString(Md.GetMethodDefinition(h).Name) == name
        && SigProvider.FullName(Md, Md.GetMethodDefinition(h).GetDeclaringType()) == type.FullName);

    private static StructureReader.BodyFacts Body(string name) => Reader.Body(Find(typeof(BodyTests), name));

    [Fact]
    public void Virtual_calls_name_the_slot_of_every_override_and_implementation()
    {
        Assert.Equal([Reader.Slot(Find(typeof(SlotDerived), "M"))], Body("CallBase").VirtualCalls);
        Assert.Equal(Reader.Slot(Find(typeof(SlotDerived), "M")), Reader.Slot(Find(typeof(GenericDerived<>), "M")));
        Assert.Equal("M`0/1", Body("CallBase").VirtualCalls.Single());
        string explicitName = Md.MethodDefinitions.Select(h => Md.GetString(Md.GetMethodDefinition(h).Name)).Single(n => n.EndsWith("ISlot<System.String>.Run"));
        Assert.Equal([Reader.Slot(Find(typeof(SlotImpl), explicitName))], Body("CallInterface").VirtualCalls);
        Assert.Equal("Run`0/1", Body("CallInterface").VirtualCalls.Single());
    }

    [Fact]
    public void Fields_are_named_by_their_definition_whatever_the_instantiation()
    {
        string field = $"{typeof(Holder<>).FullName}::Value";
        Assert.Equal([field], Body("Write").Writes);
        Assert.Equal([field], Body("ReadValue").Reads);
        Assert.Empty(Body("ReadValue").Writes);
    }

    [Fact]
    public void The_hash_follows_the_body_not_the_token_or_name()
    {
        Assert.Equal(Body("Constant").Hash, Body("Constant2").Hash);
        Assert.NotEqual(Body("Data").Hash, Body("OtherData").Hash);
    }

    [Fact]
    public void Fields_holding_a_mutable_reference_count_as_written_through()
    {
        Assert.Equal([$"{typeof(BodyTests).FullName}::Table"], Body("Fill").ReferenceReads);
        Assert.Equal([$"{typeof(BodyTests).FullName}::Label", $"{typeof(BodyTests).FullName}::Table"], Body("Fill").Reads);
        Assert.Empty(Body("Fill").Writes);
    }

    [Theory]
    [InlineData("Items")]
    [InlineData("Later")]
    public void Iterator_and_async_state_machines_belong_to_their_method(string method)
    {
        var machine = typeof(BodyTests).GetNestedTypes(System.Reflection.BindingFlags.NonPublic)
            .Single(t => t.Name.StartsWith($"<{method}>d__"));
        Assert.Equal((typeof(BodyTests).FullName!, method), CompilerGenerated.Owner(machine.FullName!, "MoveNext"));
        Assert.Contains(Md.MethodDefinitions, h => Md.GetString(Md.GetMethodDefinition(h).Name) == "MoveNext"
            && SigProvider.FullName(Md, Md.GetMethodDefinition(h).GetDeclaringType()) == machine.FullName);
    }

    [Fact]
    public void Test_types_depend_on_the_helpers_their_IL_uses_including_extension_methods()
    {
        var user = Md.TypeDefinitions.Single(h => SigProvider.FullName(Md, h) == typeof(ExtensionUser).FullName);
        var names = TestTypes.References(Md, Pe, user).Select(h => TestTypes.Name(Md, h)).ToList();
        Assert.Contains(typeof(RenderExtensions).FullName!, names);
        Assert.Contains(typeof(GenericHelper<>).FullName!, names);
        Assert.DoesNotContain(typeof(BodyTests).FullName!, names);
    }
}

public static class RenderExtensions
{
    public static string Render(this int value) => value.ToString();
}

public static class GenericHelper<T>
{
    public static List<T> Items { get; } = [];
}

public class ExtensionUser
{
    public string Show(int value) => value.Render() + GenericHelper<ExtensionUser>.Items.Count;
}
