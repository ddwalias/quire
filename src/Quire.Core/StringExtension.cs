#nullable disable
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public static partial class StringExtension
{
    [MirrorVerified("PtcStringExtensionMechanicalDifferentialTests")]
    private static Regex _invalidXmlChars;

    [MirrorVerified("PtcStringExtensionMechanicalDifferentialTests")]
    public static string StripInvalidXmlChars(this string str) => _invalidXmlChars.Replace(str, "");

    [MirrorVerified("PtcStringExtensionMechanicalDifferentialTests")]
    private static Regex _validXmlString;

    [MirrorVerified("PtcStringExtensionMechanicalDifferentialTests")]
    public static bool IsValidXmlString(this string str) => _validXmlString.IsMatch(str);

    [MirrorVerified("Release IL")]
    private static int AsciiEndCode;

    [MirrorVerified("Release IL")]
    public static bool IsAscii(char c) => c <= AsciiEndCode;

    [MirrorVerified("Release IL")]
    public static bool IsStraightQuoteChar(this char toCheck) => toCheck == '"';
}
