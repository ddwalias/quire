using System.Globalization;
using System.Text.RegularExpressions;
using Quire.Mirror;

namespace Quire;

public static partial class StringExtension
{
    [MirrorVerified("PtcStringExtensionTierAMechanicalDifferentialTests")]
    static StringExtension()
    {
        _invalidXmlChars = new Regex("[\\u0000-\\u0008\\u000B\\u000C\\u000E-\\u001F\\uD800-\\uDFFF\\uFFFE\\uFFFF]+");
        _validXmlString = new Regex("^[\\u0009\\u000A\\u000D\\u0020-\\uD7FF\\uE000-\\uFFFD]*$");
        AsciiEndCode = 127;
    }

    [MirrorVerified("PtcStringExtensionTierAMechanicalDifferentialTests")]
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

    [MirrorVerified("PtcStringExtensionTierAMechanicalDifferentialTests")]
    public static bool IsStraightQuoteChar(this char toCheck)
    {
        return toCheck == '"';
    }

    [MirrorVerified("PtcStringExtensionTierAMechanicalDifferentialTests")]
    public static bool IsAnyQuoteChar(this char toCheck)
    {
        if (!toCheck.IsStraightQuoteChar())
        {
            return toCheck.IsOpenCloseUnicodeQuoteChar();
        }
        return true;
    }

    [MirrorVerified("PtcStringExtensionTierAMechanicalDifferentialTests")]
    public static bool IsOpenCloseUnicodeQuoteChar(this char toCheck)
    {
        UnicodeCategory unicodeCategory = char.GetUnicodeCategory(toCheck);
        if (unicodeCategory != UnicodeCategory.InitialQuotePunctuation)
        {
            return unicodeCategory == UnicodeCategory.FinalQuotePunctuation;
        }
        return true;
    }
}
