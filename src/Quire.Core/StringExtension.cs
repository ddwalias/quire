#nullable disable
using System.Linq;
using System.Globalization;
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

    [MirrorVerified("Release IL")]
    public static bool ContainsUnicodeCharOrSlash(this string str) => str.Any((char ch) => !IsAscii(ch) || ch.CompareTo('\\') == 0);

    [MirrorVerified("Release IL")]
    private static int AsciiEndCode;

    [MirrorVerified("Release IL")]
    public static bool IsAscii(char c) => c <= AsciiEndCode;

    [MirrorVerified("Release IL")]
    public static bool IsStraightQuoteChar(this char toCheck) => toCheck == '"';

    public static bool IsAnyQuoteChar(this char toCheck)
    {
        if (!toCheck.IsStraightQuoteChar())
        {
            return toCheck.IsOpenCloseUnicodeQuoteChar();
        }
        return true;
    }

    public static bool IsOpenCloseUnicodeQuoteChar(this char toCheck)
    {
        UnicodeCategory unicodeCategory = char.GetUnicodeCategory(toCheck);
        if (unicodeCategory != UnicodeCategory.InitialQuotePunctuation)
        {
            return unicodeCategory == UnicodeCategory.FinalQuotePunctuation;
        }
        return true;
    }

    public static bool IsWrappedWithQuotes(this string stringToCheck, QuotesType quotesType)
    {
        if (stringToCheck.Length < 2)
        {
            return false;
        }
        switch (quotesType)
        {
            case QuotesType.Any:
                if (stringToCheck.First().IsAnyQuoteChar())
                {
                    return stringToCheck.Last().IsAnyQuoteChar();
                }
                return false;
            case QuotesType.BothStraight:
                if (stringToCheck.First().IsStraightQuoteChar())
                {
                    return stringToCheck.Last().IsStraightQuoteChar();
                }
                return false;
            case QuotesType.BothNonStraight:
                if (stringToCheck.IsWrappedWithQuotes(QuotesType.Any))
                {
                    return !stringToCheck.IsWrappedWithQuotes(QuotesType.BothStraight);
                }
                return false;
            default:
                return false;
        }
    }
}
