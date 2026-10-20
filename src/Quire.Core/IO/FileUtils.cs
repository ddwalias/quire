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
        string[] array2 = absoluteFilePath.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
        int i = 0;
        goto LoopTest;
    LoopBody:
        if (!array[i].ToLower().Equals(array2[i].ToLower()))
        {
            goto LoopDone;
        }
        i++;
    LoopTest:
        if (i < Math.Min(array.Length, array2.Length))
        {
            goto LoopBody;
        }
    LoopDone:
        if (i == 0)
        {
            return absoluteFilePath;
        }
        string text = i < array.Length ? ".." : ".";
        for (int j = i + 1; j < array.Length; j++)
        {
            text = text + Path.DirectorySeparatorChar + "..";
        }
        for (int k = i; k < array2.Length; k++)
        {
            text = text + Path.DirectorySeparatorChar + array2[k];
        }
        if (stripSingleLeadingDot)
        {
            string text2 = "." + Path.DirectorySeparatorChar;
            if (text.StartsWith(text2))
            {
                text = text.Substring(text2.Length);
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
        string text2 = "." + number + text;
        return baseFileName.Remove(baseFileName.Length - text.Length) + text2;
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
