#nullable disable
using System;
using System.IO;
using System.Reflection;
using Quire.Mirror;
using Quire.Services;

namespace Quire.IO;

public partial class FileUtils
{
    [MirrorVerified("Release IL")]
    private static string GetAppDataPathWithBase(string appDataPathBase, bool useAssemblyVersionInConfigPath = false)
    {
        Assembly element = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        string company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(element, typeof(AssemblyCompanyAttribute))).Company;
        string text = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(element, typeof(AssemblyProductAttribute))).Product;
        if (text == "PTC Mathcad Prime")
        {
            text = GetProductNamePerVariant();
        }
        string text2 = Path.Combine(Path.Combine(appDataPathBase, company), text);
        if (useAssemblyVersionInConfigPath)
        {
            string version = ((AssemblyVersionAttribute)Attribute.GetCustomAttribute(element, typeof(AssemblyVersionAttribute))).Version;
            text2 = Path.Combine(text2, version);
        }
        return text2;
    }

    [MirrorVerified("PtcIOFileUtilsMechanicalDifferentialTests")]
    public static string GetLocalConfigFolderPath(bool useAssemblyVersionInConfigPath = false) => GetLocalApplicationDataPath(useAssemblyVersionInConfigPath);

    [MirrorVerified("PtcIOFileUtilsMechanicalDifferentialTests")]
    public static string GetCommonConfigFolderPath(bool useAssemblyVersionInConfigPath = false) => GetCommonApplicationDataPath(useAssemblyVersionInConfigPath);

    [MirrorVerified("PtcIOFileUtilsMechanicalDifferentialTests")]
    public FileUtils() { }

    [MirrorVerified("Release IL")]
    public static string GetApplicationDataPath() => GetAppDataPathWithBase(Environment.GetFolderPath((Environment.SpecialFolder)26), false);

    [MirrorVerified("Release IL")]
    public static string GetLocalApplicationDataPath(bool useAssemblyVersionInConfigPath = false) => GetAppDataPathWithBase(Environment.GetFolderPath((Environment.SpecialFolder)28), useAssemblyVersionInConfigPath);

    [MirrorVerified("Release IL")]
    public static string GetCommonApplicationDataPath(bool useAssemblyVersionInConfigPath = false) => GetAppDataPathWithBase(Environment.GetFolderPath((Environment.SpecialFolder)35), useAssemblyVersionInConfigPath);
}
