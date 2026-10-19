using System.Reflection;
using System.Security.Cryptography;
using Quire.IO;

namespace Quire.Differential.Tests;

/// <summary>Directly compares Ptc.IO.EmbeddedResourceCopier.Copy with Mathcad and checks its file output.</summary>
public class EmbeddedResourceCopierMechanicalDifferentialTests
{
    private const string Contract = "Ptc.IO.EmbeddedResourceCopier";

    [SkippableFact]
    public void Copy_resource_and_missing_resource()
    {
        Mathcad.Require();
        Assembly resourceAssembly = typeof(object).Assembly;
        const string resourceName = "System.Private.CoreLib.Strings.resources";
        Assembly originalAssembly = Mathcad.Load("Ptc.Core");
        Assert.Equal("d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56",
            Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(originalAssembly.Location))).ToLowerInvariant());
        Assert.Equal(Guid.Parse("fc0806aa-db73-489f-8ae4-4729770faca6"), originalAssembly.ManifestModule.ModuleVersionId);
        Type originalType = originalAssembly.GetType("Ptc.IO.EmbeddedResourceCopier")!;
        MethodInfo quireCopy = typeof(EmbeddedResourceCopier).GetMethod(nameof(EmbeddedResourceCopier.Copy))!;
        MethodInfo originalCopy = originalType.GetMethod("Copy", BindingFlags.Public | BindingFlags.Static)!;
        Assert.Equal(0x06000f88, originalCopy.MetadataToken);
        Assert.Equal([typeof(Assembly), typeof(string), typeof(string)], originalCopy.GetParameters().Select(parameter => parameter.ParameterType));

        string root = Path.Combine(Path.GetTempPath(), "Quire.EmbeddedResourceCopier", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            string quireDestination = Path.Combine(root, "quire-resource.bin");
            string mathcadDestination = Path.Combine(root, "mathcad-resource.bin");
            byte[] resourceBytes;
            using (Stream stream = resourceAssembly.GetManifestResourceStream(resourceName)!)
            using (MemoryStream copy = new())
            {
                stream.CopyTo(copy);
                resourceBytes = copy.ToArray();
            }
            string expectedText = new(resourceBytes.Select(value => (char)value).ToArray());

            EmbeddedResourceCopier.Copy(resourceAssembly, resourceName, quireDestination);
            Assert.Equal(expectedText, File.ReadAllText(quireDestination));
            MethodComparison.Compare("existing shared resource writes byte-to-char text", Contract,
                quireCopy, null, [resourceAssembly, resourceName, quireDestination],
                originalCopy, null, [resourceAssembly, resourceName, mathcadDestination],
                value => Show(value, quireDestination), value => Show(value, mathcadDestination));

            string quireExisting = Path.Combine(root, "quire-existing.bin");
            string mathcadExisting = Path.Combine(root, "mathcad-existing.bin");
            File.WriteAllText(quireExisting, "old", System.Text.Encoding.UTF8);
            File.WriteAllText(mathcadExisting, "old", System.Text.Encoding.UTF8);
            File.SetAttributes(quireExisting, FileAttributes.ReadOnly);
            File.SetAttributes(mathcadExisting, FileAttributes.ReadOnly);
            EmbeddedResourceCopier.Copy(resourceAssembly, "missing-resource-for-differential-test", quireExisting);
            Assert.Equal("old", File.ReadAllText(quireExisting));
            Assert.Equal(FileAttributes.Normal, File.GetAttributes(quireExisting));
            MethodComparison.Compare("missing resource normalizes an existing destination and preserves it", Contract,
                quireCopy, null, [resourceAssembly, "missing-resource-for-differential-test", quireExisting],
                originalCopy, null, [resourceAssembly, "missing-resource-for-differential-test", mathcadExisting],
                value => Show(value, quireExisting), value => Show(value, mathcadExisting));
        }
        finally
        {
            foreach (string file in Directory.EnumerateFiles(root))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
            Directory.Delete(root, recursive: true);
        }
    }

    private static string Show(object? value, string destination) => value switch
    {
        Assembly assembly => "assembly:" + assembly.GetName().Name,
        string text when text == destination => "<destination>",
        null => File.Exists(destination)
            ? File.GetAttributes(destination) + ":" + Convert.ToHexString(File.ReadAllBytes(destination))
            : "<missing>",
        _ => Reflect.Text(value),
    };
}
