#nullable disable
#pragma warning disable

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
        string nearby_text;
        int num = global::Quire.Engine.Misc.FileUtils.ComputeLineNumber(str, pos, out nearby_text);
        global::System.Console.WriteLine("Bad syntax ({0}) @ pos={1} line={2}\n Context (line {2}): {3}", msg, pos, num, nearby_text);
        throw new global::System.Exception("bad_syntax");
    }
    private static void Dbg(string str)
    {
        if (s_Debug)
        {
            global::System.Console.WriteLine(str);
        }
    }
    public static bool scanExpression(string str, ref int startPos, out int endPos)
    {
        int startPos2 = startPos;
        endPos = startPos;
        if (!findExpressionStart(str, ref startPos2))
        {
            return false;
        }
        int startPos3 = startPos2;
        if (!skipExpression(str, ref startPos3))
        {
            return false;
        }
        startPos = startPos2;
        endPos = startPos3;
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
        int startPos2 = startPos;
        int num = 0;
        Dbg("skipList entered, level=0, pos=" + startPos);
        if (!findExpressionStart(str, ref startPos2))
        {
            Dbg("skipList found no start.");
            return false;
        }
        if (!str[startPos2].Equals(s_OpenParen))
        {
            Dbg("str=" + str + " pos=" + startPos2 + " ch=" + str[startPos2]);
            badSyntax("missing open paren at start of skipList", str, startPos2);
        }
        num++;
        for (startPos2++; startPos2 < str.Length; startPos2++)
        {
            char c = str[startPos2];
            Dbg("pos=" + startPos2 + " ch=" + c + " level=" + num);
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
                if (!skipLine(str, ref startPos2))
                {
                    break;
                }
                Dbg("stringList skipped comment; pos=" + startPos2 + " str[pos]=" + str[startPos2] + " level=" + num);
                startPos2--;
            }
            else if (c.Equals(s_StringQuote))
            {
                if (!skipString(str, ref startPos2))
                {
                    break;
                }
                Dbg("stringList skipped string; pos=" + startPos2 + " str[pos]=" + str[startPos2] + " level=" + num);
                startPos2--;
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
                    startPos = startPos2 + 1;
                    Dbg("Leaving skipList @ " + startPos);
                    return true;
                }
            }
        }
        Dbg("Leaving failing skipList @ " + startPos2 + "/" + str.Length);
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
    public static global::System.Collections.Generic.Queue<string> scanExpressionLine(string s)
    {
        int cursor = 0;
        return scanExpressionLine(s, ref cursor);
    }
    public static global::System.Collections.Generic.Queue<string> scanExpressionLine(string s, ref int cursor)
    {
        global::System.Collections.Generic.Queue<string> queue = new global::System.Collections.Generic.Queue<string>();
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
    public static global::System.Collections.Generic.Queue<string> scanExpressionLine(string s, ref int cursor, bool permitXML)
    {
        global::Quire.Engine.Misc.LispUtils.LinePartition linePartition = partitionExpressionLine(s, ref cursor, permitXML);
        if (linePartition != null)
        {
            return linePartition.Segments();
        }
        return null;
    }
    public static global::Quire.Engine.Misc.LispUtils.LinePartition partitionExpressionLine(string s, ref int cursor, bool permitXML)
    {
        global::Quire.Engine.Misc.LispUtils.LinePartition linePartition = new global::Quire.Engine.Misc.LispUtils.LinePartition(s);
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
    public static global::System.Collections.Generic.Queue<string> scanExpressions(string s)
    {
        return scanExpressions(s, 0);
    }
    public static global::System.Collections.Generic.Queue<string> scanExpressions(string s, int startPos)
    {
        global::System.Collections.Generic.Queue<string> queue = new global::System.Collections.Generic.Queue<string>();
        int startPos2 = startPos;
        int endPos = startPos;
        while (true)
        {
            bool flag = scanExpression(s, ref startPos2, out endPos);
            Dbg(flag ? "scanExpressions found expression" : "scanExpressions found no expression");
            if (!flag)
            {
                break;
            }
            Dbg("s=" + s);
            Dbg("i=" + startPos2);
            Dbg("e=" + endPos);
            queue.Enqueue(s.Substring(startPos2, endPos - startPos2));
            startPos2 = endPos;
        }
        return queue;
    }
    private static void PrettyPrint(string exp)
    {
        global::System.Console.WriteLine(PrettyString(exp));
    }
    public static string PrettyString(string exp)
    {
        return PrettyStringTop(exp, 0);
    }
    public static string PrettyStringTop(string exp, int target_indentation)
    {
        global::System.IO.TextWriter textWriter = new global::System.IO.StringWriter();
        int spos = 0;
        int epos = exp.Length;
        PrettyStringWorker(exp, true, ref spos, ref epos, target_indentation, textWriter);
        return textWriter.ToString();
    }
    public static bool IsDefinerName(string op_name)
    {
        foreach (string text in s_Definers)
        {
            if (op_name == text)
            {
                return true;
            }
        }
        return false;
    }
    public static string IdentifierName(string op_name)
    {
        int num = op_name.IndexOfAny(s_SymbolSeps);
        if (num >= 0)
        {
            return op_name.Substring(0, num);
        }
        return op_name;
    }
    private static void PrettyStringWorker(string exp, bool top, ref int spos, ref int epos, int indent, global::System.IO.TextWriter writer)
    {
        int num = 0;
        int num2 = global::System.Int32.MaxValue;
        bool flag = false;
        while (findExpressionStart(exp, ref spos))
        {
            if ((num > 0) & top)
            {
                throw new global::System.Exception("More than one expression");
            }
            int endPos = spos;
            if (exp[spos] == s_CloseParen)
            {
                if (top)
                {
                    throw new global::System.Exception("Extra close paren at top level.");
                }
                return;
            }
            if (exp[spos] == s_CommentChar)
            {
                throw new global::System.Exception("Pretty printer doesn't handle comments.");
            }
            if (!scanExpression(exp, ref spos, out endPos))
            {
                throw new global::System.Exception("Incomplete expression.");
            }
            if (flag)
            {
                IndentTo(indent, writer);
            }
            else if (num > 0)
            {
                writer.Write(" ");
            }
            bool flag2 = exp[spos] == s_OpenParen;
            if (flag2 && num == 0)
            {
                flag = true;
            }
            num2--;
            if (num2 <= 0)
            {
                flag = true;
            }
            if (flag2)
            {
                writer.Write(s_OpenParen);
                spos++;
                PrettyStringWorker(exp, false, ref spos, ref epos, indent + 1, writer);
                if (exp[spos] != s_CloseParen)
                {
                    throw new global::System.Exception("Missing close paren");
                }
                spos++;
                writer.Write(s_CloseParen);
            }
            else
            {
                string text = exp.Substring(spos, endPos - spos);
                string text2 = IdentifierName(text);
                if (num == 0)
                {
                    if (IsDefinerName(text2))
                    {
                        num2 = 1;
                        indent++;
                    }
                    else if (text2 == "progn")
                    {
                        flag = true;
                        indent++;
                    }
                    else if (text2 == "op_matrix")
                    {
                        num2 = 2;
                        indent++;
                    }
                    else if (text.Length > 8)
                    {
                        flag = true;
                        indent += 3;
                    }
                    else
                    {
                        num2 = 1;
                        indent = indent + text.Length + 1;
                    }
                }
                writer.Write("{0}", text);
            }
            spos = endPos;
            num++;
        }
    }
    public static void IndentTo(int n, global::System.IO.TextWriter writer)
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
        bool flag2 = false;
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
                flag2 = !flag2;
                continue;
            }
            global::System.Console.WriteLine("arg=" + text);
            try
            {
                string text2 = global::Quire.Engine.Misc.FileUtils.ExpandUnicode(text);
                if (flag)
                {
                    int cursor = 0;
                    while (true)
                    {
                        global::System.Collections.Generic.Queue<string> queue = scanExpressionLine(text2, ref cursor);
                        if (queue == null)
                        {
                            goto EndTry;
                        }
                        foreach (string item in queue)
                        {
                            global::System.Console.WriteLine(" exp=" + item);
                        }
                        global::System.Console.WriteLine("--------------------");
                    }
                }
                if (flag2)
                {
                    PrettyPrint(text2);
                    goto EndTry;
                }
                foreach (string item2 in scanExpressions(text2))
                {
                    global::System.Console.WriteLine(" exp=" + item2);
                }
            EndTry:
                ;
            }
            catch (global::System.Exception ex)
            {
                global::System.Console.WriteLine("Error: " + ex);
            }
        }
        if (flag | flag2)
        {
            return "Done.";
        }
        return "";
    }
    public LispUtils() { }

    partial class LinePartition
    {
        private string m_Line;
        private global::System.Collections.Generic.Queue<global::Quire.Engine.Misc.Pair<int, int>> m_Locs = new global::System.Collections.Generic.Queue<global::Quire.Engine.Misc.Pair<int, int>>();
        public string Line => m_Line;
        public int Count => m_Locs.Count;
        public LinePartition(string line)
        {
            m_Line = line;
        }
        public void PutSegment(int startPos, int endPos)
        {
            m_Locs.Enqueue(new global::Quire.Engine.Misc.Pair<int, int>(startPos, endPos));
        }
        public string PeekSegment()
        {
            global::Quire.Engine.Misc.Pair<int, int> pair = m_Locs.Peek();
            int first = pair.First;
            int second = pair.Second;
            return m_Line.Substring(first, second - first);
        }
        public string TakeSegment()
        {
            global::Quire.Engine.Misc.Pair<int, int> pair = m_Locs.Dequeue();
            int first = pair.First;
            int second = pair.Second;
            return m_Line.Substring(first, second - first);
        }
        public global::System.Collections.Generic.Queue<string> Segments()
        {
            return SegmentsFrom(0);
        }
        public global::System.Collections.Generic.Queue<string> SegmentsFrom(int n)
        {
            global::System.Collections.Generic.Queue<string> queue = new global::System.Collections.Generic.Queue<string>();
            int num = 0;
            foreach (global::Quire.Engine.Misc.Pair<int, int> loc in m_Locs)
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
            int num2 = m_Line.Length;
            if (n >= m_Locs.Count)
            {
                return "";
            }
            if (n < 0)
            {
                n = 0;
            }
            int num3 = -1;
            foreach (global::Quire.Engine.Misc.Pair<int, int> loc in m_Locs)
            {
                if (++num3 == n)
                {
                    num = loc.First;
                }
                num2 = loc.Second;
            }
            return m_Line.Substring(num, num2 - num);
        }
    }
}
