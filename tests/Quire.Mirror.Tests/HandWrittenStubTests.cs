using System;
using System.IO;
using System.Linq;
using Quire.Tools.MirrorGen;

namespace Quire.Mirror.Tests;

public class HandWrittenStubTests
{
    [Fact]
    public void HandWritten_stubs_are_preserved_but_remain_unfinished()
    {
        string root = Path.Combine(Path.GetTempPath(), "mirrorgen-stub-" + Guid.NewGuid().ToString("N"));
        string project = "Fixture";
        string projectDir = Path.Combine(root, "src", project);
        string output = Path.Combine(projectDir, "bin", "Release", "net10.0");
        Directory.CreateDirectory(output);
        File.Copy(typeof(StubFixture).Assembly.Location, Path.Combine(output, project + ".dll"));
        File.WriteAllText(Path.Combine(projectDir, "Fixture.cs"), "namespace Quire.Mirror.Tests; class StubFixture { }");
        try
        {
            var model = MetadataLoader.Load(Path.Combine(output, project + ".dll"))!;
            var fixture = model.Types.Single(t => t.FullName == typeof(StubFixture).FullName);
            var manual = fixture.Methods.Single(m => m.Name == nameof(StubFixture.Manual));
            var generated = fixture.Methods.Single(m => m.Name == nameof(StubFixture.Generated));
            Assert.True(manual.Stub);
            Assert.True(manual.HandWrittenStub);
            Assert.True(generated.Stub);
            Assert.False(generated.HandWrittenStub);

            var query = new TypeModel
            {
                Assembly = project, Project = project, FullName = fixture.FullName, Namespace = fixture.Namespace,
                Name = fixture.Name, Kind = fixture.Kind, Access = fixture.Access
            };
            var hand = HandWritten.Load(root, [project]);
            Assert.Contains(HandWritten.MethodKey(manual), hand.MethodKeys(query));
            Assert.DoesNotContain(HandWritten.MethodKey(generated), hand.MethodKeys(query));
            var constructor = fixture.Methods.Single(m => m.Name == ".ctor");
            Assert.True(constructor.Stub);
            Assert.True(constructor.HandWrittenStub);
            Assert.Contains(HandWritten.MethodKey(constructor), hand.MethodKeys(query));
            Assert.Contains("ManualField", hand.NonMethodNames(query));
            Assert.DoesNotContain("GeneratedField", hand.NonMethodNames(query));
            Assert.Contains("ManualProperty", hand.NonMethodNames(query));
            Assert.DoesNotContain("GeneratedProperty", hand.NonMethodNames(query));
            Assert.Contains("ManualEvent", hand.NonMethodNames(query));
            Assert.DoesNotContain("GeneratedEvent", hand.NonMethodNames(query));
            Assert.True(fixture.Properties.Single(p => p.Name == "ManualProperty").Stub);
            Assert.True(fixture.Events.Single(e => e.Name == "ManualEvent").Stub);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

public class StubFixture
{
    [MirrorBlocked("compile order"), MirrorStub] public StubFixture() { }
    [MirrorStub, MirrorBlocked("compile order")] public int ManualField;
    [MirrorStub] public int GeneratedField;
    [MirrorStub, MirrorBlocked("compile order")] public int ManualProperty { get; set; }
    [MirrorStub] public int GeneratedProperty { get; set; }
    [MirrorStub, MirrorBlocked("compile order")] public event Action? ManualEvent;
    [MirrorStub] public event Action? GeneratedEvent;
    [MirrorStub, MirrorBlocked("compile order")] public void Manual() { }
    [MirrorStub] public void Generated() { }
}
