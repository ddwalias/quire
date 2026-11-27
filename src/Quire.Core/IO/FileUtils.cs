#nullable disable
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Quire.Mirror;
using Quire.Services;

namespace Quire.IO;

public partial class FileUtils
{
    [MirrorVerified("Release IL")]
    public static string EvaluateRelativePath(string mainDirPath, string absoluteFilePath, bool stripSingleLeadingDot)
    {
        string[] array = mainDirPath.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
        string[] absolutePathSegments = absoluteFilePath.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
        int i = 0;
        while (i < Math.Min(array.Length, absolutePathSegments.Length))
        {
            if (!array[i].ToLower().Equals(absolutePathSegments[i].ToLower()))
            {
                break;
            }
            i++;
        }
        if (i == 0)
        {
            return absoluteFilePath;
        }
        string text = i < array.Length ? ".." : ".";
        for (int j = i + 1; j < array.Length; j++)
        {
            text = text + Path.DirectorySeparatorChar + "..";
        }
        for (int k = i; k < absolutePathSegments.Length; k++)
        {
            text = text + Path.DirectorySeparatorChar + absolutePathSegments[k];
        }
        if (stripSingleLeadingDot)
        {
            string relativePathPrefix = "." + Path.DirectorySeparatorChar;
            if (text.StartsWith(relativePathPrefix))
            {
                text = text.Substring(relativePathPrefix.Length);
            }
        }
        return text;
    }

    [MirrorVerified("Release IL")]
    public static bool IsLocalIpAddress(string host)
    {
        try
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
            IPAddress[] hostAddresses2 = Dns.GetHostAddresses(Dns.GetHostName());
            return hostAddresses.Any(IPAddress.IsLoopback) || hostAddresses.Intersect(hostAddresses2).IsNotEmpty();
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
            return false;
        }
    }

    [MirrorVerified("Release IL")]
    public static string GetAssemblyFolder(Assembly asm)
    {
        if (asm == null)
        {
            return null;
        }
        Uri uri = new Uri(asm.CodeBase);
        return Path.GetDirectoryName(uri.LocalPath);
    }

    [MirrorVerified("Release IL")]
    public static string GetFileNameWithNumber(string baseFileName, int number, bool numberIsLastExtension)
    {
        DebugTools.DevAssert(number >= 0, null);
        if (number == 0)
        {
            return baseFileName;
        }
        IFileOperationsProvider fileOperationsProvider = new FileOperationsProvider();
        string text = numberIsLastExtension ? string.Empty : fileOperationsProvider.GetExtension(baseFileName);
        string numberedSuffix = "." + number + text;
        return baseFileName.Remove(baseFileName.Length - text.Length) + numberedSuffix;
    }

    [MirrorVerified("Release IL")]
    public static string GetNextNumberedFileName(string folder, string baseFileName, bool numberIsLastExtension)
    {
        string foundFileName;
        int number = 1 + FindHighestNumberedFileName(folder, baseFileName, numberIsLastExtension, out foundFileName);
        string fileNameWithNumber = GetFileNameWithNumber(baseFileName, number, numberIsLastExtension);
        return Path.Combine(folder, fileNameWithNumber);
    }

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
        string appDataPath = Path.Combine(Path.Combine(appDataPathBase, company), text);
        if (useAssemblyVersionInConfigPath)
        {
            string version = ((AssemblyVersionAttribute)Attribute.GetCustomAttribute(element, typeof(AssemblyVersionAttribute))).Version;
            appDataPath = Path.Combine(appDataPath, version);
        }
        return appDataPath;
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
