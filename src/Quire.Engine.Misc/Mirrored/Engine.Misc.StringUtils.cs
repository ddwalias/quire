#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Text;

namespace Quire.Engine.Misc;

partial class StringUtils
{
    public static char[] Whitespace = new char[] { ' ', '\t' };

    private static char[] s_CommandSpecialChars = new char[] { '\\', '"', '\'', '\t', ' ' };

    public static string Capitalize(string s)
    {
        if (s.Length == 0)
        {
            return s;
        }
        return string.Format("{0}{1}", s[0].ToString().ToUpper(), s.Substring(1, s.Length - 1));
    }

    public static string CharName(char ch)
    {
        if (ch == ' ') return "Space";
        if (ch == '\t') return "Tab";
        if (ch == '\f') return "Form";
        if (ch == '\r') return "Return";
        if (ch == '\n') return "Newline";
        return new string(ch, 1);
    }

    public static string PrefixLines(string s, string prefix)
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (s.Length > 0)
        {
            stringBuilder.Append(prefix);
        }
        foreach (char value in s)
        {
            if (value.Equals('\n'))
            {
                stringBuilder.AppendFormat("\n{0}", prefix);
            }
            else
            {
                stringBuilder.Append(value);
            }
        }
        return stringBuilder.ToString();
    }

    public static bool ContainsString(string candidate, string[] set)
    {
        foreach (string s in set)
        {
            if (s.Equals(candidate))
            {
                return true;
            }
        }
        return false;
    }

    public static string CharNames(string str)
    {
        StringBuilder stringBuilder = new StringBuilder();
        bool flag = true;
        foreach (char ch in str)
        {
            if (flag)
            {
                flag = false;
            }
            else
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.Append(CharName(ch));
        }
        return stringBuilder.ToString();
    }

    public static string StringSubstitute(string str, string substr, string replacement)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int num = 0;
        int length = substr.Length;
        int matchIndex;
        while ((matchIndex = str.IndexOf(substr, num)) >= 0)
        {
            stringBuilder.Append(str, num, matchIndex - num);
            stringBuilder.Append(replacement);
            num = matchIndex + length;
        }
        stringBuilder.Append(str, num, str.Length - num);
        return stringBuilder.ToString();
    }

    public static bool SameLines(string leftText, string rightText)
    {
        return SameLines(leftText, rightText, 0, leftText.Length, 0, rightText.Length);
    }

    private static bool IsWhitespace(char ch)
    {
        if (ch != ' ')
        {
            return ch == '\t';
        }
        return true;
    }

    public static bool SameLines(string leftText, string rightText, int leftStart, int leftEnd, int rightStart, int rightEnd)
    {
        List<string> list = new List<string>();
        List<string> rightLines = new List<string>();
        foreach (string leftLine in leftText.Split('\n', StringSplitOptions.None))
        {
            string leftTrimmed = leftLine.Trim();
            if (leftTrimmed != "")
            {
                list.Add(leftTrimmed);
            }
        }
        foreach (string rightLine in rightText.Split('\n', StringSplitOptions.None))
        {
            string rightTrimmed = rightLine.Trim();
            if (rightTrimmed != "")
            {
                rightLines.Add(rightTrimmed);
            }
        }
        if (list.Count != rightLines.Count)
        {
            return false;
        }
        int count = list.Count;
        for (int j = 0; j < count; j++)
        {
            if (list[j] != rightLines[j])
            {
                return false;
            }
        }
        return true;
    }

    public static string StringQuote(string str)
    {
        return StringQuote(str, '"', '\\');
    }

    public static string StringQuote(string str, char quoter)
    {
        return StringQuote(str, quoter, '\\');
    }

    public static string StringQuote(string str, char quoter, char escaper)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append('"');
        foreach (char c in str)
        {
            if (c == quoter || c == escaper)
            {
                stringBuilder.Append(escaper);
            }
            stringBuilder.Append(c);
        }
        stringBuilder.Append('"');
        return stringBuilder.ToString();
    }

    public static string StringUnquote(string str)
    {
        int length = str.Length;
        if (length < 2)
        {
            return str;
        }
        if (!str.StartsWith("\"") || !str.EndsWith("\""))
        {
            return str;
        }
        return ExpandStringEscapes(str.Substring(1, length - 2));
    }

    public static string ExpandStringEscapes(string str)
    {
        StringBuilder stringBuilder = new StringBuilder();
        new StringBuilder();
        bool flag = false;
        foreach (char c in str)
        {
            if (flag)
            {
                flag = false;
                switch (c)
                {
                    case 'f':
                        stringBuilder.Append('\f');
                        break;
                    case 't':
                        stringBuilder.Append('\t');
                        break;
                    case 'r':
                        stringBuilder.Append('\f');
                        break;
                    case 'n':
                        stringBuilder.Append('\n');
                        break;
                    case '\\':
                        stringBuilder.Append('\\');
                        break;
                    case '\'':
                        stringBuilder.Append('\'');
                        break;
                    case '"':
                        stringBuilder.Append('"');
                        break;
                    default:
                        throw new Exception(string.Format("Bad char after backslash: {0}", c));
                }
            }
            else if (c.Equals('\\'))
            {
                flag = true;
            }
            else
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString();
    }

    public StringUtils() { }
}

partial class StringUtils
{
    public static string ToCommandString(string[] args)
    {
        var stringBuilder = new StringBuilder();
        foreach (string text in args)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.AppendFormat(text.IndexOfAny(s_CommandSpecialChars) >= 0 ? StringQuote(text) : text);
        }
        return stringBuilder.ToString();
    }
}
