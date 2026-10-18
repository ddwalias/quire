#nullable disable
using System.Text.RegularExpressions;
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
}
