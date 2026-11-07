#nullable disable
using System;
using System.Windows.Documents;

namespace Quire.Controls.Core;

public partial class FindUtilities
{
    public const StringComparison CaseSensitiveStringComparison = StringComparison.Ordinal;

    public const StringComparison CaseInsensitiveStringComparison = StringComparison.OrdinalIgnoreCase;

    public static int? GetIndexInText(string haystack, string needle, LogicalDirection direction, FindOptions options)
    {
        StringComparison comparisonType = options.HasFlag(FindOptions.MatchCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        DelegateFunction1<int, int> delegateFunction2;
        DelegateFunction1<bool, int> delegateFunction = (int i) => i > -1 && i < haystack.Length;
        DelegateFunction1<int, int> delegateFunction3;
        int num;
        if (LogicalDirection.Forward == direction)
        {
            delegateFunction2 = (int i) => haystack.IndexOf(needle, i, comparisonType);
            delegateFunction3 = (int i) => ++i;
            num = 0;
        }
        else
        {
            delegateFunction2 = (int i) => haystack.LastIndexOf(needle, i, comparisonType);
            delegateFunction3 = (int i) => i + needle.Length - 2;
            num = haystack.Length;
        }
        bool flag;
        do
        {
            num = delegateFunction2(num);
            flag = -1 != num;
            if (flag && (options & FindOptions.WholeWordOnly) != 0 && !(flag = IsWholeWord(haystack, num, needle.Length)))
            {
                num = delegateFunction3(num);
            }
        }
        while (!flag && delegateFunction(num));
        if (!flag)
        {
            return null;
        }
        return num;
    }

    private static bool IsWholeWord(string text, int start, int length)
    {
        int num = start + length;
        if (start == 0 || !char.IsLetterOrDigit(text, start - 1))
        {
            if (text.Length != num)
            {
                return !char.IsLetterOrDigit(text, num);
            }
            return true;
        }
        return false;
    }

    public static Func<string, int> CreateSearchFunctionForGivenString(string valueToFind, bool isCaseSensitiveMatch, bool isWholeWordOnly)
    {
        StringComparison comparisonType = isCaseSensitiveMatch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return (string value) =>
        {
            if (!isWholeWordOnly)
            {
                return value.IndexOf(valueToFind, comparisonType);
            }
            if (!value.Equals(valueToFind, comparisonType))
            {
                return -1;
            }
            return 0;
        };
    }

    public static Func<string, int> CreateCaseInsensitiveSearchFunctionForGivenString(string valueToFind, string valueToReplaceWith, bool isWholeWordOnly)
    {
        Func<string, int> baseSearchFunc = CreateSearchFunctionForGivenString(valueToFind, false, isWholeWordOnly);
        return (string value) =>
        {
            int num = 0;
            while (num < value.Length)
            {
                int num2 = baseSearchFunc(value.Substring(num));
                if (num2 == -1)
                {
                    return -1;
                }
                int num3 = num + num2;
                if (value.Substring(num3, valueToFind.Length).Equals(valueToReplaceWith, StringComparison.Ordinal))
                {
                    num = num3 + 1;
                }
                else
                {
                    return num3;
                }
            }
            return -1;
        };
    }

    public FindUtilities()
    {
    }
}
