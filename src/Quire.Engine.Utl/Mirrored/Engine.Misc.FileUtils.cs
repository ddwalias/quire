#nullable disable
#pragma warning disable

namespace Quire.Engine.Misc;

partial class FileUtils
{
    public static global::System.IO.StreamReader StreamToPath(string path)
    {
        return new global::System.IO.StreamReader(path, global::System.Text.Encoding.UTF8, true);
    }
    public static void ReadExpanded(string path, global::System.IO.TextWriter _out)
    {
        global::System.Collections.Generic.Stack<global::System.IO.StreamReader> stack = new global::System.Collections.Generic.Stack<global::System.IO.StreamReader>();
        global::System.Collections.Generic.Stack<string> stack2 = new global::System.Collections.Generic.Stack<string>();
        global::System.Collections.Generic.Stack<bool> stack3 = new global::System.Collections.Generic.Stack<bool>();
        try
        {
            global::System.IO.StreamReader streamReader = StreamToPath(path);
            bool flag = false;
            int num = 0;
            stack.Push(streamReader);
            stack2.Push(path);
            stack3.Push(flag);
            while (streamReader != null)
            {
                string text = streamReader.ReadLine();
                if (text != null)
                {
                    if (text.IndexOf("/*") >= 0 && text.Trim() == "/*")
                    {
                        num++;
                    }
                    else if (text.IndexOf("*/") >= 0 && text.Trim() == "*/")
                    {
                        num--;
                    }
                    else
                    {
                        if (num != 0)
                        {
                            continue;
                        }
                        string text2 = ProcessInclude(text, out var isReference);
                        if (text2 != null)
                        {
                            string directoryName = global::System.IO.Path.GetDirectoryName(path);
                            if (directoryName == null)
                            {
                                continue;
                            }
                            string text3 = global::System.IO.Path.Combine(directoryName, text2);
                            if (directoryName != null)
                            {
                                streamReader = StreamToPath(text3);
                                path = text3;
                                flag = isReference;
                                stack.Push(streamReader);
                                stack2.Push(text3);
                                stack3.Push(flag);
                                if (flag)
                                {
                                    SkipPast(streamReader, "### END PREAMBLE ###");
                                }
                            }
                        }
                        else if (flag && text.StartsWith("### BEGIN TEST CASE"))
                        {
                            SkipPast(streamReader, "### END TEST CASE");
                        }
                        else
                        {
                            _out.WriteLine(text);
                        }
                    }
                }
                else
                {
                    if (num > 0)
                    {
                        throw new global::System.Exception(string.Format("End of file within comment.", default(global::System.ReadOnlySpan<object>)));
                    }
                    stack.Pop();
                    stack2.Pop();
                    stack3.Pop();
                    streamReader.Dispose();
                    if (stack.Count > 0)
                    {
                        streamReader = stack.Peek();
                        path = stack2.Peek();
                        flag = stack3.Peek();
                    }
                    else
                    {
                        streamReader = null;
                        path = null;
                        flag = false;
                    }
                }
            }
        }
        finally
        {
            while (stack.Count > 0)
            {
                stack.Pop().Dispose();
            }
        }
    }
    public static string ReadExpanded(string path)
    {
        global::System.IO.StringWriter stringWriter = new global::System.IO.StringWriter();
        ReadExpanded(path, stringWriter);
        stringWriter.Close();
        return stringWriter.ToString();
    }
    private static string ProcessInclude(string line, out bool isReference)
    {
        int length = line.Length;
        int i;
        for (i = 0; length > i && (line[i] == ' ' || line[i] == '	'); i++)
        {
        }
        string text = ((i > 0) ? line.Substring(i) : line);
        if (text.StartsWith("#include") || text.StartsWith("#reference"))
        {
            isReference = text.StartsWith("#reference");
            int num = text.IndexOf('"');
            int num2 = text.LastIndexOf('"');
            if (num == -1 || num2 - num < 2)
            {
                throw new global::System.Exception("bad #include directive");
            }
            return text.Substring(num + 1, num2 - num - 1);
        }
        isReference = false;
        return null;
    }
    private static void SkipPast(global::System.IO.StreamReader r, string seek)
    {
        string text;
        do
        {
            text = r.ReadLine();
        }
        while (text != null && !text.StartsWith(seek));
    }
    public static string ContractUnicode(string str)
    {
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            uint num = c;
            char ascii = '\0';
            string charName = null;
            if (num < 128)
            {
                stringBuilder.Append(c);
            }
            else if (UnicodeToAscii(c, ref ascii))
            {
                stringBuilder.AppendFormat("\\{0}", ascii);
            }
            else if (UnicodeToGreekShortcutKey(c, ref ascii))
            {
                stringBuilder.AppendFormat("\\g{0}", ascii);
            }
            else if (LongUnicodeToShortcutKey(str, ref i, ref ascii))
            {
                stringBuilder.AppendFormat("\\{0}", ascii);
            }
            else if (UnicodeToCharName(c, ref charName))
            {
                stringBuilder.AppendFormat("\\&{0};", charName);
            }
            else
            {
                stringBuilder.AppendFormat("\\u{0:x4}", num);
            }
        }
        return stringBuilder.ToString();
    }
    public static string ExpandUnicode(string str)
    {
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
        global::System.Text.StringBuilder stringBuilder2 = new global::System.Text.StringBuilder();
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        int num = 0;
        while (num < str.Length)
        {
            char c = str[num];
            num++;
            char unicode = '\0';
            if (flag)
            {
                flag = false;
                if (AsciiToUnicode(c, ref unicode))
                {
                    stringBuilder.Append(unicode);
                    continue;
                }
                switch (c)
                {
                case 'f':
                    stringBuilder.Append('\f');
                    break;
                case 't':
                    stringBuilder.Append('\t');
                    break;
                case 'r':
                    stringBuilder.Append('\r');
                    break;
                case 'n':
                    stringBuilder.Append('\n');
                    break;
                case '\r':
                    if (num < str.Length && str[num] == '\n')
                    {
                        num++;
                    }
                    break;
                case '\t':
                case ' ':
                {
                    int num2 = num;
                    while (num2 < str.Length)
                    {
                        char c2 = str[num2];
                        num2++;
                        if (c2 == '\n')
                        {
                            break;
                        }
                        if (c2 == '\r')
                        {
                            if (num2 < str.Length && str[num2] == '\n')
                            {
                                num2++;
                            }
                        }
                        else if (c2 != ' ' && c2 != '\t')
                        {
                            throw new global::System.Exception(global::System.String.Format("Bad escape sequence: {0}", StringUtils.CharNames(str.Substring(num - 2, num2 - num + 2))));
                        }
                        else
                        {
                            continue;
                        }
                        break;
                    }
                    num = num2;
                    break;
                }
                case '[':
                    stringBuilder.Append('\u0086');
                    stringBuilder.Append('₊');
                    break;
                case ']':
                    stringBuilder.Append('\u0087');
                    stringBuilder.Append('₊');
                    break;
                case '(':
                    stringBuilder.Append('\u0086');
                    stringBuilder.Append('⁺');
                    break;
                case ')':
                    stringBuilder.Append('\u0087');
                    stringBuilder.Append('⁺');
                    break;
                case 'g':
                    flag2 = true;
                    break;
                case '&':
                    flag4 = true;
                    break;
                case 'u':
                    flag3 = true;
                    break;
                default:
                    stringBuilder.AppendFormat("\\{0}", c);
                    break;
                case '\n':
                    break;
                }
            }
            else if (flag2)
            {
                flag2 = false;
                char unicode2 = '\0';
                if (GreekShortcutKeyToUnicode(c, ref unicode2))
                {
                    stringBuilder.Append(unicode2);
                    continue;
                }
                num--;
                stringBuilder.AppendFormat("\\g{0}", c);
            }
            else if (flag3)
            {
                bool flag5 = false;
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                    stringBuilder2.Append(c);
                    if (stringBuilder2.Length == 4)
                    {
                        flag5 = true;
                    }
                }
                else
                {
                    num--;
                    stringBuilder.AppendFormat("\\u{0}", stringBuilder2);
                    stringBuilder2.Length = 0;
                    flag3 = false;
                }
                if (flag5)
                {
                    stringBuilder.Append((char)global::System.Int32.Parse(stringBuilder2.ToString(), global::System.Globalization.NumberStyles.AllowHexSpecifier));
                    stringBuilder2.Length = 0;
                    flag3 = false;
                }
            }
            else if (flag4)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_')
                {
                    stringBuilder2.Append(c);
                    continue;
                }
                string text = stringBuilder2.ToString();
                stringBuilder2.Length = 0;
                flag4 = false;
                if (c == ';')
                {
                    string unicode3 = null;
                    if (CharNameToUnicodeString(text, ref unicode3))
                    {
                        stringBuilder.Append(unicode3);
                    }
                    else
                    {
                        stringBuilder.AppendFormat("\\&{0};", text);
                    }
                }
                else
                {
                    num--;
                    stringBuilder.AppendFormat("\\&{0}", text);
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
    public static bool UnicodeToAscii(char unicode, ref char ascii)
    {
        if (s_UnicodeToAsciiTable.Count == 0)
        {
            InitCharMappings();
        }
        if (s_UnicodeToAsciiTable.ContainsKey(unicode))
        {
            ascii = s_UnicodeToAsciiTable[unicode];
            return true;
        }
        return false;
    }
    public static bool AsciiToUnicode(char ascii, ref char unicode)
    {
        if (s_AsciiToUnicodeTable.Count == 0)
        {
            InitCharMappings();
        }
        if (s_AsciiToUnicodeTable.ContainsKey(ascii))
        {
            unicode = s_AsciiToUnicodeTable[ascii];
            return true;
        }
        return false;
    }
    public static bool CharNameToUnicodeString(string name, ref string unicode)
    {
        if (!s_bNamedCharMapInitialized)
        {
            InitNamedCharMap();
        }
        if (s_CharNameToUnicodeStringTable.ContainsKey(name))
        {
            unicode = s_CharNameToUnicodeStringTable[name];
            return true;
        }
        return false;
    }
    public static bool UnicodeToCharName(char unicode, ref string charName)
    {
        if (!s_bNamedCharMapInitialized)
        {
            InitNamedCharMap();
        }
        if (s_UnicodeToCharNameTable.ContainsKey(unicode))
        {
            charName = s_UnicodeToCharNameTable[unicode];
            return true;
        }
        return false;
    }
    public static bool GreekShortcutKeyToUnicode(char greekShortcut, ref char unicode)
    {
        if (!s_bNamedCharMapInitialized)
        {
            InitNamedCharMap();
        }
        if (s_GreekShortcutKeyToUnicodeTable.ContainsKey(greekShortcut))
        {
            unicode = s_GreekShortcutKeyToUnicodeTable[greekShortcut];
            return true;
        }
        return false;
    }
    public static bool LongUnicodeToShortcutKey(string str, ref int pos, ref char shortcutKey)
    {
        if (!s_bNamedCharMapInitialized)
        {
            InitNamedCharMap();
        }
        bool flag = false;
        foreach (global::Quire.Engine.Misc.Pair<string, char> item in s_LongUnicodeToShortcutKeyTable)
        {
            string first = item.First;
            char second = item.Second;
            if (str.Length >= pos + first.Length)
            {
                bool flag2 = false;
                for (int i = 0; i < first.Length; i++)
                {
                    if (str[pos + i] != first[i])
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    shortcutKey = second;
                    pos += first.Length - 1;
                    flag = true;
                    break;
                }
            }
        }
        return flag;
    }
    public static bool UnicodeToGreekShortcutKey(char unicode, ref char greekShortcut)
    {
        if (!s_bNamedCharMapInitialized)
        {
            InitNamedCharMap();
        }
        if (s_UnicodeToGreekShortcutKeyTable.ContainsKey(unicode))
        {
            greekShortcut = s_UnicodeToGreekShortcutKeyTable[unicode];
            return true;
        }
        return false;
    }
    public static int ComputeLineNumber(string str, int pos)
    {
        string nearby_text;
        return ComputeLineNumber(str, pos, out nearby_text);
    }
    public static int ComputeLineNumber(string str, int pos, out string nearby_text)
    {
        string pointer_text;
        return ComputeLineNumber(str, pos, out nearby_text, out pointer_text);
    }
    public static int ComputeLineNumber(string str, int pos, out string nearby_text, out string pointer_text)
    {
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
        global::System.Text.StringBuilder stringBuilder2 = new global::System.Text.StringBuilder();
        int num = 1;
        int num2 = 0;
        int num3 = pos;
        if (num3 < 0)
        {
            num3 = 0;
        }
        if (num3 > str.Length)
        {
            num3 = str.Length;
        }
        while (num2 < num3)
        {
            char c = str[num2];
            if (c == '\n')
            {
                num++;
                stringBuilder.Length = 0;
                stringBuilder2.Length = 0;
            }
            else
            {
                stringBuilder2.Append(' ');
                stringBuilder.Append(c);
            }
            num2++;
        }
        stringBuilder2.Append("^");
        if (num2 < str.Length)
        {
            while (num2 < str.Length)
            {
                char c2 = str[num2];
                if (c2 == '\n' || c2 == '\r')
                {
                    break;
                }
                stringBuilder.Append(c2);
                num2++;
            }
        }
        nearby_text = stringBuilder.ToString();
        pointer_text = stringBuilder2.ToString();
        return num;
    }
    public FileUtils() { }
}
