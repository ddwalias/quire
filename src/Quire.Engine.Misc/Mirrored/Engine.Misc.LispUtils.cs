#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.IO;

namespace Quire.Engine.Misc;

partial class LispUtils
{
    private static bool s_Debug = false;
    private static string s_LispWhitepaceChars = " \t\f\n\r";
    private static string s_LispHorizontalWhitespaceChars = " \t";
    private static string s_LispVerticalWhitespaceChars = "\f\n\r";
    private static string s_LispBreakChars = " \t\f\n\r()\";";
    private static char s_EscapeChar = '\\';
    private static char s_OpenParen = '(';
    private static char s_CloseParen = ')';
    private static char s_StringQuote = '"';
    private static char s_CommentChar = ';';
    private static char[] s_SymbolSeps = "{:".ToCharArray();
    private static string[] s_Definers = "define|global|define-eval|lambda".Split("|".ToCharArray());
    public static void badSyntax(string msg, string str, int pos)
    {
        string nearbyText;
        int num = FileUtils.ComputeLineNumber(str, pos, out nearbyText);
        Console.WriteLine("Bad syntax ({0}) @ pos={1} line={2}\n Context (line {2}): {3}", msg, pos, num, nearbyText);
        throw new Exception("bad_syntax");
    }
    private static void Dbg(string str)
    {
        if (s_Debug)
        {
            Console.WriteLine(str);
        }
    }
    public static bool scanExpression(string str, ref int startPos, out int endPos)
    {
        int exprStart = startPos;
        endPos = startPos;
        if (!findExpressionStart(str, ref exprStart))
        {
            return false;
        }
        int exprEnd = exprStart;
        if (!skipExpression(str, ref exprEnd))
        {
            return false;
        }
        startPos = exprStart;
        endPos = exprEnd;
        return true;
    }
    public static bool skipExpression(string str, ref int startPos)
    {
        Dbg("str=" + str + " startPos=" + startPos);
        if (!findExpressionStart(str, ref startPos))
        {
            Dbg("skipExpression found no start.");
            return false;
        }
        if (str[startPos].Equals(s_OpenParen))
        {
            return skipList(str, ref startPos);
        }
        return skipAtom(str, ref startPos);
    }
    public static bool skipList(string str, ref int startPos)
    {
        bool flag = false;
        int cursor = startPos;
        int num = 0;
        Dbg("skipList entered, level=0, pos=" + startPos);
        if (!findExpressionStart(str, ref cursor))
        {
            Dbg("skipList found no start.");
            return false;
        }
        if (!str[cursor].Equals(s_OpenParen))
        {
            Dbg("str=" + str + " pos=" + cursor + " ch=" + str[cursor]);
            badSyntax("missing open paren at start of skipList", str, cursor);
        }
        num++;
        for (cursor++; cursor < str.Length; cursor++)
        {
            char c = str[cursor];
            Dbg("pos=" + cursor + " ch=" + c + " level=" + num);
            if (flag)
            {
                flag = false;
            }
            else if (c.Equals(s_EscapeChar))
            {
                flag = true;
            }
            else if (c.Equals(s_CommentChar))
            {
                if (!skipLine(str, ref cursor))
                {
                    break;
                }
                Dbg("stringList skipped comment; pos=" + cursor + " str[pos]=" + str[cursor] + " level=" + num);
                cursor--;
            }
            else if (c.Equals(s_StringQuote))
            {
                if (!skipString(str, ref cursor))
                {
                    break;
                }
                Dbg("stringList skipped string; pos=" + cursor + " str[pos]=" + str[cursor] + " level=" + num);
                cursor--;
            }
            else if (c.Equals(s_OpenParen))
            {
                num++;
            }
            else if (c.Equals(s_CloseParen))
            {
                num--;
                if (num == 0)
                {
                    startPos = cursor + 1;
                    Dbg("Leaving skipList @ " + startPos);
                    return true;
                }
            }
        }
        Dbg("Leaving failing skipList @ " + cursor + "/" + str.Length);
        return false;
    }
    public static bool skipAtom(string str, ref int startPos)
    {
        bool flag = false;
        int i = startPos;
        Dbg("skipAtom entered, pos=" + startPos);
        if (!findExpressionStart(str, ref i))
        {
            Dbg("skipAtom found no start.");
            return false;
        }
        if (str[i] == s_StringQuote)
        {
            if (!skipString(str, ref i))
            {
                return false;
            }
            startPos = i;
            Dbg("Leaving skipAtom after string @ " + startPos);
            return true;
        }
        if (s_LispBreakChars.IndexOf(str[i]) != -1)
        {
            Dbg("str=" + str + " pos=" + i + " ch=" + str[i]);
            badSyntax("skipAtom found no start of atom", str, i);
        }
        for (; i < str.Length; i++)
        {
            char value = str[i];
            if (flag)
            {
                flag = false;
            }
            else if (value.Equals(s_EscapeChar))
            {
                flag = true;
            }
            else if (s_LispBreakChars.IndexOf(value) != -1)
            {
                startPos = i;
                Dbg("Leaving skipAtom @ " + startPos);
                return true;
            }
        }
        if (!flag && i != startPos)
        {
            Dbg("Leaving skipAtom @ eof=" + startPos);
            startPos = i;
            return true;
        }
        Dbg("Leaving failing skipAtom @ " + i);
        return false;
    }
    public static bool findExpressionStart(string str, ref int startPos)
    {
        return findExpressionStart(str, ref startPos, s_LispWhitepaceChars);
    }
    public static bool findExpressionStart(string str, ref int startPos, string whitespace)
    {
        for (int i = startPos; i < str.Length; i++)
        {
            char value = str[i];
            if (value.Equals(s_CommentChar))
            {
                if (!skipLine(str, ref i))
                {
                    break;
                }
                i--;
                continue;
            }
            int num = whitespace.IndexOf(value);
            Dbg("ch=" + value + " wspos=" + num);
            if (num == -1)
            {
                num = s_LispWhitepaceChars.IndexOf(value);
                if (num == -1)
                {
                    startPos = i;
                    Dbg("findExpressionStart succeeded @ " + i);
                    return true;
                }
                startPos = i;
                Dbg("findExpressionStart whitespace barrier @ " + i);
                return false;
            }
        }
        Dbg("findExpressionStart failing");
        return false;
    }
    public static bool skipLine(string str, ref int startPos)
    {
        int i;
        for (i = startPos; i < str.Length; i++)
        {
            if (str[i].Equals('\n'))
            {
                startPos = i + 1;
                return true;
            }
        }
        startPos = i;
        return false;
    }
    public static bool skipString(string str, ref int startPos)
    {
        Dbg("skipString entered @" + startPos);
        int i = startPos + 1;
        if (!str[startPos].Equals(s_StringQuote))
        {
            badSyntax("missing string quote in skipString", str, startPos);
        }
        for (; i < str.Length; i++)
        {
            char c = str[i];
            if (c.Equals(s_EscapeChar))
            {
                i++;
            }
            else if (c.Equals(s_StringQuote))
            {
                startPos = i + 1;
                Dbg("skipString accepting position " + startPos);
                return true;
            }
        }
        Dbg("skipString failing @ " + i);
        return false;
    }
    public static Queue<string> scanExpressionLine(string s)
    {
        int cursor = 0;
        return scanExpressionLine(s, ref cursor);
    }
    public static Queue<string> scanExpressionLine(string s, ref int cursor)
    {
        Queue<string> queue = new Queue<string>();
        int startPos = cursor;
        int endPos = cursor;
        if (!findExpressionStart(s, ref startPos))
        {
            return null;
        }
        while (findExpressionStart(s, ref startPos, s_LispHorizontalWhitespaceChars))
        {
            endPos = startPos;
            if (s_LispVerticalWhitespaceChars.IndexOf(s[startPos]) >= 0)
            {
                skipLine(s, ref startPos);
                break;
            }
            if (scanExpression(s, ref startPos, out endPos))
            {
                queue.Enqueue(s.Substring(startPos, endPos - startPos));
                startPos = endPos;
            }
            else
            {
                badSyntax("ill-formed expression", s, startPos);
            }
        }
        cursor = endPos;
        return queue;
    }
    public static Queue<string> scanExpressionLine(string s, ref int cursor, bool permitXML)
    {
        LispUtils.LinePartition linePartition = partitionExpressionLine(s, ref cursor, permitXML);
        if (linePartition != null)
        {
            return linePartition.Segments();
        }
        return null;
    }
    public static LispUtils.LinePartition partitionExpressionLine(string s, ref int cursor, bool permitXML)
    {
        LispUtils.LinePartition linePartition = new LispUtils.LinePartition(s);
        int startPos = cursor;
        int endPos = cursor;
        Dbg(string.Format("* Looking at {0}", s[startPos]));
        if (!findExpressionStart(s, ref startPos))
        {
            return null;
        }
        Dbg(string.Format("** Looking at {0}", s[startPos]));
        while (findExpressionStart(s, ref startPos, s_LispHorizontalWhitespaceChars))
        {
            Dbg(string.Format("*** Looking at {0}", s[startPos]));
            endPos = startPos;
            if (s_LispVerticalWhitespaceChars.IndexOf(s[startPos]) >= 0)
            {
                skipLine(s, ref startPos);
                endPos = startPos;
                break;
            }
            if (s[startPos].Equals('<'))
            {
                Dbg("Saw <");
                if (XmlUtils.ScanElement(s, ref startPos, out endPos))
                {
                    linePartition.PutSegment(startPos, endPos);
                    startPos = endPos;
                }
                else
                {
                    badSyntax("ill-formed xml", s, startPos);
                }
            }
            else if (scanExpression(s, ref startPos, out endPos))
            {
                linePartition.PutSegment(startPos, endPos);
                startPos = endPos;
            }
            else
            {
                badSyntax("ill-formed expression", s, startPos);
            }
        }
        cursor = endPos;
        return linePartition;
    }
    public static Queue<string> scanExpressions(string s)
    {
        return scanExpressions(s, 0);
    }
    public static Queue<string> scanExpressions(string s, int startPos)
    {
        Queue<string> queue = new Queue<string>();
        int cursor = startPos;
        int endPos = startPos;
        while (true)
        {
            bool flag = scanExpression(s, ref cursor, out endPos);
            Dbg(flag ? "scanExpressions found expression" : "scanExpressions found no expression");
            if (!flag)
            {
                break;
            }
            Dbg("s=" + s);
            Dbg("i=" + cursor);
            Dbg("e=" + endPos);
            queue.Enqueue(s.Substring(cursor, endPos - cursor));
            cursor = endPos;
        }
        return queue;
    }
    private static void PrettyPrint(string exp)
    {
        Console.WriteLine(PrettyString(exp));
    }
    public static string PrettyString(string exp)
    {
        return PrettyStringTop(exp, 0);
    }
    public static string PrettyStringTop(string exp, int targetIndentation)
    {
        TextWriter textWriter = new StringWriter();
        int spos = 0;
        int epos = exp.Length;
        PrettyStringWorker(exp, true, ref spos, ref epos, targetIndentation, textWriter);
        return textWriter.ToString();
    }
    public static bool IsDefinerName(string opName)
    {
        foreach (string text in s_Definers)
        {
            if (opName == text)
            {
                return true;
            }
        }
        return false;
    }
    public static string IdentifierName(string opName)
    {
        int num = opName.IndexOfAny(s_SymbolSeps);
        if (num >= 0)
        {
            return opName.Substring(0, num);
        }
        return opName;
    }
    private static void PrettyStringWorker(string exp, bool top, ref int spos, ref int epos, int indent, TextWriter writer)
    {
        int count = 0;
        int remaining = Int32.MaxValue;
        bool startNewLine = false;
        while (findExpressionStart(exp, ref spos))
        {
            if ((count > 0) & top)
            {
                throw new Exception("More than one expression");
            }
            int endPos = spos;
            if (exp[spos] == s_CloseParen)
            {
                if (top)
                {
                    throw new Exception("Extra close paren at top level.");
                }
                return;
            }
            if (exp[spos] == s_CommentChar)
            {
                throw new Exception("Pretty printer doesn't handle comments.");
            }
            if (!scanExpression(exp, ref spos, out endPos))
            {
                throw new Exception("Incomplete expression.");
            }
            if (startNewLine)
            {
                IndentTo(indent, writer);
            }
            else if (count > 0)
            {
                writer.Write(" ");
            }
            bool isList = exp[spos] == s_OpenParen;
            if (isList && count == 0)
            {
                startNewLine = true;
            }
            remaining--;
            if (remaining <= 0)
            {
                startNewLine = true;
            }
            if (isList)
            {
                writer.Write(s_OpenParen);
                spos++;
                PrettyStringWorker(exp, false, ref spos, ref epos, indent + 1, writer);
                if (exp[spos] != s_CloseParen)
                {
                    throw new Exception("Missing close paren");
                }
                spos++;
                writer.Write(s_CloseParen);
            }
            else
            {
                string text = exp.Substring(spos, endPos - spos);
                string head = IdentifierName(text);
                if (count == 0)
                {
                    if (IsDefinerName(head))
                    {
                        remaining = 1;
                        indent++;
                    }
                    else if (head == "progn")
                    {
                        startNewLine = true;
                        indent++;
                    }
                    else if (head == "op_matrix")
                    {
                        remaining = 2;
                        indent++;
                    }
                    else if (text.Length > 8)
                    {
                        startNewLine = true;
                        indent += 3;
                    }
                    else
                    {
                        remaining = 1;
                        indent = indent + text.Length + 1;
                    }
                }
                writer.Write("{0}", text);
            }
            spos = endPos;
            count++;
        }
    }
    public static void IndentTo(int n, TextWriter writer)
    {
        writer.Write("\n");
        for (int i = 0; i < n; i++)
        {
            writer.Write(" ");
        }
    }
    public static string SelfTest(string[] args)
    {
        bool flag = false;
        bool prettyPrintMode = false;
        foreach (string text in args)
        {
            if (text.Equals("--selftest"))
            {
                continue;
            }
            if (text.Equals("-o"))
            {
                flag = !flag;
                continue;
            }
            if (text.Equals("-d"))
            {
                s_Debug = !s_Debug;
                continue;
            }
            if (text.Equals("-pp"))
            {
                prettyPrintMode = !prettyPrintMode;
                continue;
            }
            Console.WriteLine("arg=" + text);
            try
            {
                string expanded = FileUtils.ExpandUnicode(text);
                if (flag)
                {
                    int cursor = 0;
                    while (true)
                    {
                        Queue<string> queue = scanExpressionLine(expanded, ref cursor);
                        if (queue == null)
                        {
                            // Release IL requires this branch to skip the remaining mode handlers.
                            goto EndTry;
                        }
                        foreach (string item in queue)
                        {
                            Console.WriteLine(" exp=" + item);
                        }
                        Console.WriteLine("--------------------");
                    }
                }
                if (prettyPrintMode)
                {
                    PrettyPrint(expanded);
                    // Release IL requires this branch to skip expression scanning.
                    goto EndTry;
                }
                foreach (string expression in scanExpressions(expanded))
                {
                    Console.WriteLine(" exp=" + expression);
                }
            EndTry:
                ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
        if (flag | prettyPrintMode)
        {
            return "Done.";
        }
        return "";
    }
    public LispUtils() { }

    partial class LinePartition
    {
        private string m_Line;
        private Queue<Pair<int, int>> m_Locs = new Queue<Pair<int, int>>();
        public string Line => m_Line;
        public int Count => m_Locs.Count;
        public LinePartition(string line)
        {
            m_Line = line;
        }
        public void PutSegment(int startPos, int endPos)
        {
            m_Locs.Enqueue(new Pair<int, int>(startPos, endPos));
        }
        public string PeekSegment()
        {
            Pair<int, int> pair = m_Locs.Peek();
            int first = pair.First;
            int second = pair.Second;
            return m_Line.Substring(first, second - first);
        }
        public string TakeSegment()
        {
            Pair<int, int> pair = m_Locs.Dequeue();
            int first = pair.First;
            int second = pair.Second;
            return m_Line.Substring(first, second - first);
        }
        public Queue<string> Segments()
        {
            return SegmentsFrom(0);
        }
        public Queue<string> SegmentsFrom(int n)
        {
            Queue<string> queue = new Queue<string>();
            int num = 0;
            foreach (Pair<int, int> loc in m_Locs)
            {
                if (num >= n)
                {
                    int first = loc.First;
                    int second = loc.Second;
                    queue.Enqueue(m_Line.Substring(first, second - first));
                }
            }
            return queue;
        }
        public string JoinedText()
        {
            return JoinedTextFrom(0);
        }
        public string JoinedTextFrom(int n)
        {
            int num = m_Line.Length;
            int end = m_Line.Length;
            if (n >= m_Locs.Count)
            {
                return "";
            }
            if (n < 0)
            {
                n = 0;
            }
            int index = -1;
            foreach (Pair<int, int> loc in m_Locs)
            {
                if (++index == n)
                {
                    num = loc.First;
                }
                end = loc.Second;
            }
            return m_Line.Substring(num, end - num);
        }
    }
}
