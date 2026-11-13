using Quire.Tools.MirrorGen;

namespace Quire.Mirror.Tests;

public class HandWrittenPackageDependencyTests
{
    [Fact]
    public void Handwritten_using_adds_package_without_mirrored_reference()
    {
        string root = Path.Combine(Path.GetTempPath(), "mirrorgen-package-" + Guid.NewGuid().ToString("N"));
        string project = "Fixture";
        string source = Path.Combine(root, "src", project, "Excel", "ExcelReader.cs");
        Directory.CreateDirectory(Path.GetDirectoryName(source)!);
        File.WriteAllText(source, "using DocumentFormat.OpenXml.Packaging;\nclass ExcelReader { }\n");
        try
        {
            Package package = Assert.Single(Emit.HandWrittenPackages(root, project));
            Assert.Equal("DocumentFormat.OpenXml", package.Id);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
