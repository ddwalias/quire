using System.Reflection;
using System.Security.Cryptography;
using Quire.IO;

namespace Quire.Differential.Tests;

/// <summary>Direct Mathcad comparisons for Ptc.IO.FileOperationsProvider.CreateDirectory.</summary>
public class PtcIOFileOperationsProviderTierAMechanicalDifferentialTests
{
    private const string Contract = "Ptc.IO.FileOperationsProvider";

    [SkippableFact]
    public void CreateDirectory_creates_paths_and_returns_failures()
    {
        Mathcad.Require();
        Assembly originalAssembly = Mathcad.Load("Ptc.Core");
        Assert.Equal("d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56",
            Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(originalAssembly.Location))).ToLowerInvariant());
        Assert.Equal(Guid.Parse("fc0806aa-db73-489f-8ae4-4729770faca6"), originalAssembly.ManifestModule.ModuleVersionId);
        Type originalType = originalAssembly.GetType("Ptc.IO.FileOperationsProvider")!;
        MethodInfo quireCreate = typeof(FileOperationsProvider).GetMethod(nameof(FileOperationsProvider.CreateDirectory))!;
        MethodInfo originalCreate = originalType.GetMethod("CreateDirectory", BindingFlags.Public | BindingFlags.Instance)!;
        Assert.Equal(0x06000f94, originalCreate.MetadataToken);
        Assert.Equal(typeof(Exception), originalCreate.ReturnType);
        Assert.Equal([typeof(string)], originalCreate.GetParameters().Select(parameter => parameter.ParameterType));

        string root = Path.Combine(Path.GetTempPath(), "Quire.FileOperationsProvider", Guid.NewGuid().ToString("N"));
        string quireRoot = Path.Combine(root, "quire");
        string originalRoot = Path.Combine(root, "mathcad");
        Directory.CreateDirectory(quireRoot);
        Directory.CreateDirectory(originalRoot);
        object quireTarget = new FileOperationsProvider();
        object originalTarget = originalType.GetConstructor(Type.EmptyTypes)!.Invoke([]);
        try
        {
            string quireExisting = Path.Combine(quireRoot, "existing");
            string originalExisting = Path.Combine(originalRoot, "existing");
            Directory.CreateDirectory(quireExisting);
            Directory.CreateDirectory(originalExisting);
            MethodComparison.Compare("existing directory", Contract,
                quireCreate, quireTarget, [quireExisting],
                originalCreate, originalTarget, [originalExisting],
                PathShow(quireRoot), PathShow(originalRoot), State, State);

            string quireCreated = Path.Combine(quireRoot, "new", "nested");
            string originalCreated = Path.Combine(originalRoot, "new", "nested");
            MethodComparison.Compare("new nested directory", Contract,
                quireCreate, quireTarget, [quireCreated],
                originalCreate, originalTarget, [originalCreated],
                PathShow(quireRoot), PathShow(originalRoot), State, State);
            Assert.True(Directory.Exists(quireCreated));
            Assert.True(Directory.Exists(originalCreated));

            MethodComparison.Compare("null path returns its exception", Contract,
                quireCreate, quireTarget, [null],
                originalCreate, originalTarget, [null], ShowException, ShowException, State, State);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string State(object target) => Reflect.Text(Reflect.Get(target, "IsConnected"));

    private static Func<object?, string> PathShow(string root) => value => value switch
    {
        Exception exception => ShowException(exception),
        string path when path.StartsWith(root, StringComparison.OrdinalIgnoreCase) => "<root>" + path[root.Length..],
        _ => Reflect.Text(value),
    };

    private static string ShowException(object? value) => value is Exception exception
        ? exception.GetType().FullName + ": " + exception.Message
        : Reflect.Text(value);
}
