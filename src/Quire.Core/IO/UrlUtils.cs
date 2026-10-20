#nullable disable
using System;
using System.IO;
using System.Text.RegularExpressions;
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
