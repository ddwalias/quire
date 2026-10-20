#nullable disable
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Quire.Mirror;

namespace Quire.IO;

public static partial class UrlUtils
{
    [MirrorVerified("Release IL")]
    public static string GetStringAddressFromUri(Uri uri)
    {
        if (uri == null)
        {
            return null;
        }
        try
        {
            return uri.OriginalString;
        }
        catch (InvalidOperationException)
        {
            return uri.ToString();
        }
    }

    [MirrorVerified("Release IL")]
    private static int FindTagIndex(string url)
    {
        Match match = Regex.Match(url, "\\.[a-zA-Z]+#");
        if (match.Success)
        {
            return match.Index + match.Length - 1;
        }
        return -1;
    }

    [MirrorVerified("Release IL")]
    public static bool HasTag(string url, out string filePathWithoutTag, out string tagName)
    {
        filePathWithoutTag = (tagName = string.Empty);
        if (url.IsNullOrEmpty())
        {
            return false;
        }
        int num = FindTagIndex(url);
        if (num != -1)
        {
            tagName = url.Substring(num);
        }
        if (tagName.IsNullOrEmpty())
        {
            return false;
        }
        filePathWithoutTag = url.Substring(0, num);
        tagName = tagName.Substring("#".Length);
        if (!filePathWithoutTag.Contains(".mcdx") && !filePathWithoutTag.Contains(".mctx"))
        {
            tagName = string.Empty;
            return false;
        }
        return true;
    }

    [MirrorVerified("Release IL")]
    private static string makeValidExtension(string url)
    {
        string extension = Path.GetExtension(url);
        if (extension != null && extension.Contains(mathcad15Extenstion))
        {
            int num = url.IndexOf(mathcad15Extenstion);
            url = url.Substring(0, num + mathcad15Extenstion.Length);
        }
        return url;
    }

    [MirrorVerified("Release IL")]
    public static bool TryCreateAbsoluteUri(string basePath, string path, out Uri absoluteUri)
    {
        absoluteUri = null;
        path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out Uri result))
        {
            return TryCreateAbsoluteUri(basePath, result, out absoluteUri);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public static bool TryCreateAbsoluteUri(string basePath, Uri relativeUri, out Uri absoluteUri)
    {
        absoluteUri = null;
        if (relativeUri.IsAbsoluteUri)
        {
            absoluteUri = relativeUri;
            return true;
        }
        if (basePath == null)
        {
            return false;
        }
        if (!basePath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
        {
            basePath += Path.AltDirectorySeparatorChar;
        }
        if (Uri.TryCreate(basePath, UriKind.Absolute, out Uri result))
        {
            return Uri.TryCreate(result, relativeUri, out absoluteUri);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    internal static bool TryParseBrowserPath(string commandLine, out string path)
    {
        path = string.Empty;
        string[] array = commandLine.Split('"');
        string text = array[0].Trim();
        if (!text.IsNullOrEmpty())
        {
            path = text.Split(default(ReadOnlySpan<char>))[0];
        }
        else if (array.Length > 2)
        {
            path = array[1];
        }
        return !path.IsNullOrEmpty();
    }

    [MirrorVerified("Release IL")]
    private static Uri CreateUriForScheme(string text, UriProtocolCreationData uriCreationData)
    {
        try
        {
            Uri result;
            return (uriCreationData.MatchPattern.IsMatch(text) && Uri.TryCreate(uriCreationData.SchemeWithDelimiter + text, UriKind.Absolute, out result)) ? result : null;
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
            return null;
        }
    }

    [MirrorVerified("Release IL")]
    public static bool TryOpenUrl(string url, bool useBrowser, bool assert = true)
    {
        Exception ex = null;
        bool flag = false;
        try
        {
            if (useBrowser)
            {
                string standardBrowserPath = GetStandardBrowserPath();
                if (standardBrowserPath != null)
                {
                    Process.Start(standardBrowserPath, url);
                    return true;
                }
            }
            else
            {
                string fileName = makeValidExtension(url);
                Process.Start(new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = true
                });
                return true;
            }
        }
        catch (FileNotFoundException ex2)
        {
            flag = assert;
            ex = ex2;
        }
        catch (Win32Exception ex3)
        {
            flag = assert;
            ex = ex3;
        }
        catch (ObjectDisposedException ex4)
        {
            flag = true;
            ex = ex4;
        }
        catch (InvalidOperationException ex5)
        {
            flag = true;
            ex = ex5;
        }
        catch (Exception ex6)
        {
            flag = true;
            ex = ex6;
        }
        if ((ex != null) & flag)
        {
            DebugTools.FailAssertion(ex);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    private static string GetStandardBrowserPath()
    {
        string text = null;
        try
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice", writable: false))
            {
                if (registryKey != null)
                {
                    object value = registryKey.GetValue("ProgId");
                    if (value != null)
                    {
                        text = RetriveBrowserPath(Registry.ClassesRoot, value?.ToString() + "\\shell\\open\\command");
                    }
                }
            }
            if (text == null)
            {
                text = RetriveBrowserPath(Registry.CurrentUser, "Software\\Classes\\http\\shell\\open\\command")
                    ?? RetriveBrowserPath(Registry.LocalMachine, "Software\\Classes\\http\\shell\\open\\command");
            }
        }
        catch (Exception)
        {
            DebugTools.DevAssert(false, "Failed to retrieve path to default browser from registry");
        }
        return text;
    }

    [MirrorVerified("Release IL")]
    private static string RetriveBrowserPath(RegistryKey root, string regKeyPath)
    {
        if (!regKeyPath.IsNullOrEmpty())
        {
            using RegistryKey registryKey = root.OpenSubKey(regKeyPath, writable: false);
            if (registryKey != null)
            {
                string text = registryKey.GetValue(null).ToString();
                if (!text.IsNullOrEmpty() && TryParseBrowserPath(text, out string path))
                {
                    return path;
                }
            }
        }
        return null;
    }

    partial class UriProtocols
    {
        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        private static readonly HashSet<string> Protocols;

        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        public static bool Contains(string element) => Protocols.Contains(element);
    }

    partial struct UriProtocolCreationData
    {
        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        public UriProtocolCreationData(string schemeWithDelimiter, Regex matchPattern)
        {
            SchemeWithDelimiter = schemeWithDelimiter;
            MatchPattern = matchPattern;
        }

        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        public Regex MatchPattern { get; }

        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        public string SchemeWithDelimiter { get; }
    }
}
