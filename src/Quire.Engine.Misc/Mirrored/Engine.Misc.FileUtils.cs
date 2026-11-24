#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Quire.Engine.Misc;

partial class FileUtils
{
    private static string s_Char_Alpha = "Α";
    private static string s_Char_alpha = "α";
    private static string s_Char_Beta = "Β";
    private static string s_Char_beta = "β";
    private static string s_Char_Gamma = "Γ";
    private static string s_Char_gamma = "γ";
    private static string s_Char_Delta = "Δ";
    private static string s_Char_delta = "δ";
    private static string s_Char_Epsilon = "Ε";
    private static string s_Char_epsilon = "ε";
    private static string s_Char_Zeta = "Ζ";
    private static string s_Char_zeta = "ζ";
    private static string s_Char_Eta = "Η";
    private static string s_Char_eta = "η";
    private static string s_Char_Theta = "Θ";
    private static string s_Char_theta = "θ";
    private static string s_Char_Iota = "Ι";
    private static string s_Char_iota = "ι";
    private static string s_Char_Kappa = "Κ";
    private static string s_Char_kappa = "κ";
    private static string s_Char_Lambda = "Λ";
    private static string s_Char_lambda = "λ";
    private static string s_Char_Mu = "Μ";
    private static string s_Char_mu = "μ";
    private static string s_Char_Nu = "Ν";
    private static string s_Char_nu = "ν";
    private static string s_Char_Xi = "Ξ";
    private static string s_Char_xi = "ξ";
    private static string s_Char_Omicron = "Ο";
    private static string s_Char_omicron = "ο";
    private static string s_Char_Pi = "Π";
    private static string s_Char_pi = "π";
    private static string s_Char_Rho = "Ρ";
    private static string s_Char_rho = "ρ";
    private static string s_Char_Sigma = "Σ";
    private static string s_Char_sigma = "σ";
    private static string s_Char_Tau = "Τ";
    private static string s_Char_tau = "τ";
    private static string s_Char_Upsilon = "Υ";
    private static string s_Char_upsilon = "υ";
    private static string s_Char_Phi = "Φ";
    private static string s_Char_phi = "φ";
    private static string s_Char_Chi = "Χ";
    private static string s_Char_chi = "χ";
    private static string s_Char_Psi = "Ψ";
    private static string s_Char_psi = "ψ";
    private static string s_Char_Omega = "Ω";
    private static string s_Char_omega = "ω";
    private static string s_Char_phi_alt = "ϕ";
    private static string s_Char_pi_alt = "ϖ";
    private static string s_Char_sigma_alt = "ς";
    private static string s_Char_theta_alt = "ϑ";
    private static string s_Char_bottom = "⊥";
    private static string s_Char_ohm = "Ω";
    private static string s_Char_currency = "¤";
    private static string s_Char_yen = "¥";
    private static string s_Char_deg = "°";
    private static string s_Char_hbar = "ℏ";
    private static string s_Char_infinity = "∞";
    private static string s_Char_element_of = "∈";
    private static string s_Char_hooked_f = "ƒ";
    private static string s_Char_right_arrow = "→";
    private static string s_Char_left_arrow = "←";
    private static string s_Char_copyright = "©";
    private static string s_Char_trademark = "™";
    private static string s_Char_registered = "®";
    private static string s_Char_DoubleStruckC = "ℂ";
    private static string s_Char_DoubleStruckQ = "ℚ";
    private static string s_Char_DoubleStruckR = "ℝ";
    private static string s_Char_DoubleStruckZ = "ℤ";
    private static string s_Start_Subscript = "\u0086₊";
    private static string s_End_Subscript = "\u0087₊";
    private static string s_Start_Superscript = "\u0086₊";
    private static string s_End_Superscript = "\u0087₊";
    private static char[] s_CharMappings = new char[48]
    {
        'e', '∈', 'h', 'ℏ', 'o', '°', 'p', 'π', 'w', 'ω',
        'A', '∀', 'C', 'ℂ', 'E', '∃', 'O', 'Ω', 'Q', 'ℚ',
        'R', 'ℝ', 'V', '∇', 'Y', '¥', 'Z', 'ℤ', '$', '¤',
        '8', '∞', '#', '≠', '<', '≤', '>', '≥', '^', '∧',
        'v', '∨', '!', '¬', '~', '≈', '=', '≡'
    };
    private static Dictionary<char, char> s_AsciiToUnicodeTable = new Dictionary<char, char>();
    private static Dictionary<char, char> s_UnicodeToAsciiTable = new Dictionary<char, char>();
    private static readonly string[] s_NameMappings = new string[296]
    {
        "a", null, s_Char_alpha, "alpha", "A", null, s_Char_Alpha, "Alpha", "b", null,
        s_Char_beta, "beta", "B", null, s_Char_Beta, "Beta", "c", null, s_Char_chi, "chi",
        "C", null, s_Char_Chi, "Chi", "d", null, s_Char_delta, "delta", "D", null,
        s_Char_Delta, "Delta", "e", null, s_Char_epsilon, "epsilon", "E", null, s_Char_Epsilon, "Epsilon",
        "f", null, s_Char_phi, "phi", "F", null, s_Char_Phi, "Phi", "g", null,
        s_Char_gamma, "gamma", "G", null, s_Char_Gamma, "Gamma", "h", null, s_Char_eta, "eta",
        "H", null, s_Char_Eta, "Eta", "i", null, s_Char_iota, "iota", "I", null,
        s_Char_Iota, "Iota", "j", null, s_Char_phi_alt, "phi_alt", "J", null, s_Char_theta_alt, "theta_alt",
        "k", null, s_Char_kappa, "kappa", "K", null, s_Char_Kappa, "Kappa", "l", null,
        s_Char_lambda, "lambda", "L", null, s_Char_Lambda, "Lambda", "m", null, s_Char_mu, "mu",
        "M", null, s_Char_Mu, "Mu", "n", null, s_Char_nu, "nu", "N", null,
        s_Char_Nu, "Nu", "o", null, s_Char_omicron, "omicron", "O", null, s_Char_Omicron, "Omicron",
        "p", null, s_Char_pi, "pi", "P", null, s_Char_Pi, "Pi", "q", null,
        s_Char_theta, "theta", "Q", null, s_Char_Theta, "Theta", "r", null, s_Char_rho, "rho",
        "R", null, s_Char_Rho, "Rho", "s", null, s_Char_sigma, "sigma", "S", null,
        s_Char_Sigma, "Sigma", "t", null, s_Char_tau, "tau", "T", null, s_Char_Tau, "Tau",
        "u", null, s_Char_upsilon, "upsilon", "U", null, s_Char_Upsilon, "Upsilon", "v", null,
        s_Char_pi_alt, "pi_alt", "V", null, s_Char_sigma_alt, "sigma_alt", "w", null, s_Char_omega, "omega",
        "W", null, s_Char_Omega, "Omega", "x", null, s_Char_xi, "xi", "X", null,
        s_Char_Xi, "Xi", "y", null, s_Char_psi, "psi", "Y", null, s_Char_Psi, "Psi",
        "z", null, s_Char_zeta, "zeta", "Z", null, s_Char_Zeta, "Zeta", null, null,
        s_Char_currency, "currency", null, null, s_Char_ohm, "ohm", null, null, s_Char_yen, "yen",
        null, null, s_Char_deg, "deg", null, null, s_Char_infinity, "infinity", null, null,
        s_Char_hbar, "hbar", null, null, s_Char_element_of, "element_of", null, null, s_Char_bottom, "bottom",
        null, null, s_Char_right_arrow, "right_arrow", null, null, s_Char_left_arrow, "left_arrow", null, null,
        s_Char_copyright, "copyright", null, null, s_Char_trademark, "trademark", null, null, s_Char_registered, "registered",
        null, null, s_Char_hooked_f, "hooked_f", null, null, s_Char_DoubleStruckC, "DoubleStruckC", null, null,
        s_Char_DoubleStruckQ, "DoubleStruckQ", null, null, s_Char_DoubleStruckR, "DoubleStruckR", null, null, s_Char_DoubleStruckZ, "DoubleStruckZ",
        null, "[", s_Start_Subscript, "StartSubscript", null, "]", s_End_Subscript, "EndSubscript", null, "(",
        s_Start_Superscript, "StartSuperscript", null, ")", s_End_Superscript, "EndSuperscript"
    };
    private static Dictionary<string, string> s_CharNameToUnicodeStringTable = new Dictionary<string, string>();
    private static Dictionary<char, string> s_UnicodeToCharNameTable = new Dictionary<char, string>();
    private static List<Pair<string, char>> s_LongUnicodeToShortcutKeyTable = new List<Pair<string, char>>();
    private static Dictionary<char, char> s_GreekShortcutKeyToUnicodeTable = new Dictionary<char, char>();
    private static Dictionary<char, char> s_UnicodeToGreekShortcutKeyTable = new Dictionary<char, char>();
    private static bool s_bNamedCharMapInitialized = false;
}

partial class FileUtils
{
    public static StreamReader StreamToPath(string path)
    {
        return new StreamReader(path, Encoding.UTF8, true);
    }
    public static void ReadExpanded(string path, TextWriter _out)
    {
        Stack<StreamReader> stack = new Stack<StreamReader>();
        Stack<string> pathStack = new Stack<string>();
        Stack<bool> preambleFlags = new Stack<bool>();
        try
        {
            StreamReader streamReader = StreamToPath(path);
            bool inPreamble = false;
            int commentDepth = 0;
            stack.Push(streamReader);
            pathStack.Push(path);
            preambleFlags.Push(inPreamble);
            while (streamReader != null)
            {
                string text = streamReader.ReadLine();
                if (text != null)
                {
                    if (text.IndexOf("/*") >= 0 && text.Trim() == "/*")
                    {
                        commentDepth++;
                    }
                    else if (text.IndexOf("*/") >= 0 && text.Trim() == "*/")
                    {
                        commentDepth--;
                    }
                    else
                    {
                        if (commentDepth != 0)
                        {
                            continue;
                        }
                        string includePath = ProcessInclude(text, out var isReference);
                        if (includePath != null)
                        {
                            string directoryName = Path.GetDirectoryName(path);
                            if (directoryName == null)
                            {
                                continue;
                            }
                            string includedPath = Path.Combine(directoryName, includePath);
                            if (directoryName != null)
                            {
                                streamReader = StreamToPath(includedPath);
                                path = includedPath;
                                inPreamble = isReference;
                                stack.Push(streamReader);
                                pathStack.Push(includedPath);
                                preambleFlags.Push(inPreamble);
                                if (inPreamble)
                                {
                                    SkipPast(streamReader, "### END PREAMBLE ###");
                                }
                            }
                        }
                        else if (inPreamble && text.StartsWith("### BEGIN TEST CASE"))
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
                    if (commentDepth > 0)
                    {
                        throw new Exception(string.Format("End of file within comment."));
                    }
                    stack.Pop();
                    pathStack.Pop();
                    preambleFlags.Pop();
                    streamReader.Dispose();
                    if (stack.Count > 0)
                    {
                        streamReader = stack.Peek();
                        path = pathStack.Peek();
                        inPreamble = preambleFlags.Peek();
                    }
                    else
                    {
                        streamReader = null;
                        path = null;
                        inPreamble = false;
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
        StringWriter stringWriter = new StringWriter();
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
            int openQuote = text.IndexOf('"');
            int closeQuote = text.LastIndexOf('"');
            if (openQuote == -1 || closeQuote - openQuote < 2)
            {
                throw new Exception("bad #include directive");
            }
            return text.Substring(openQuote + 1, closeQuote - openQuote - 1);
        }
        isReference = false;
        return null;
    }
    private static void SkipPast(StreamReader r, string seek)
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
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            uint code = c;
            char ascii = '\0';
            string charName = null;
            if (code < 128)
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
                stringBuilder.AppendFormat("\\u{0:x4}", code);
            }
        }
        return stringBuilder.ToString();
    }
    public static string ExpandUnicode(string str)
    {
        StringBuilder stringBuilder = new StringBuilder();
        StringBuilder pending = new StringBuilder();
        bool isEscaped = false;
        bool isGreekEscape = false;
        bool isUnicodeEscape = false;
        bool isNamedEscape = false;
        int index = 0;
        while (index < str.Length)
        {
            char c = str[index];
            index++;
            char converted = '\0';
            if (isEscaped)
            {
                isEscaped = false;
                if (AsciiToUnicode(c, ref converted))
                {
                    stringBuilder.Append(converted);
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
                    if (index < str.Length && str[index] == '\n')
                    {
                        index++;
                    }
                    break;
                case '\t':
                case ' ':
                {
                    int scan = index;
                    while (scan < str.Length)
                    {
                        char scanChar = str[scan];
                        scan++;
                        if (scanChar == '\n')
                        {
                            break;
                        }
                        if (scanChar == '\r')
                        {
                            if (scan < str.Length && str[scan] == '\n')
                            {
                                scan++;
                            }
                        }
                        else if (scanChar != ' ' && scanChar != '\t')
                        {
                            throw new Exception(String.Format("Bad escape sequence: {0}", StringUtils.CharNames(str.Substring(index - 2, scan - index + 2))));
                        }
                        else
                        {
                            continue;
                        }
                        break;
                    }
                    index = scan;
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
                    isGreekEscape = true;
                    break;
                case '&':
                    isNamedEscape = true;
                    break;
                case 'u':
                    isUnicodeEscape = true;
                    break;
                default:
                    stringBuilder.AppendFormat("\\{0}", c);
                    break;
                case '\n':
                    break;
                }
            }
            else if (isGreekEscape)
            {
                isGreekEscape = false;
                char greekChar = '\0';
                if (GreekShortcutKeyToUnicode(c, ref greekChar))
                {
                    stringBuilder.Append(greekChar);
                    continue;
                }
                index--;
                stringBuilder.AppendFormat("\\g{0}", c);
            }
            else if (isUnicodeEscape)
            {
                bool hexDigitsDone = false;
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                    pending.Append(c);
                    if (pending.Length == 4)
                    {
                        hexDigitsDone = true;
                    }
                }
                else
                {
                    index--;
                    stringBuilder.AppendFormat("\\u{0}", pending);
                    pending.Length = 0;
                    isUnicodeEscape = false;
                }
                if (hexDigitsDone)
                {
                    stringBuilder.Append((char)Int32.Parse(pending.ToString(), NumberStyles.AllowHexSpecifier));
                    pending.Length = 0;
                    isUnicodeEscape = false;
                }
            }
            else if (isNamedEscape)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_')
                {
                    pending.Append(c);
                    continue;
                }
                string text = pending.ToString();
                pending.Length = 0;
                isNamedEscape = false;
                if (c == ';')
                {
                    string charNameValue = null;
                    if (CharNameToUnicodeString(text, ref charNameValue))
                    {
                        stringBuilder.Append(charNameValue);
                    }
                    else
                    {
                        stringBuilder.AppendFormat("\\&{0};", text);
                    }
                }
                else
                {
                    index--;
                    stringBuilder.AppendFormat("\\&{0}", text);
                }
            }
            else if (c.Equals('\\'))
            {
                isEscaped = true;
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
        bool found = false;
        foreach (Pair<string, char> item in s_LongUnicodeToShortcutKeyTable)
        {
            string first = item.First;
            char second = item.Second;
            if (str.Length >= pos + first.Length)
            {
                bool mismatch = false;
                for (int i = 0; i < first.Length; i++)
                {
                    if (str[pos + i] != first[i])
                    {
                        mismatch = true;
                        break;
                    }
                }
                if (!mismatch)
                {
                    shortcutKey = second;
                    pos += first.Length - 1;
                    found = true;
                    break;
                }
            }
        }
        return found;
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
        string nearbyText;
        return ComputeLineNumber(str, pos, out nearbyText);
    }
    public static int ComputeLineNumber(string str, int pos, out string nearbyText)
    {
        string pointerText;
        return ComputeLineNumber(str, pos, out nearbyText, out pointerText);
    }
    public static int ComputeLineNumber(string str, int pos, out string nearbyText, out string pointerText)
    {
        StringBuilder nearby = new StringBuilder();
        StringBuilder pointer = new StringBuilder();
        int lineNumber = 1;
        int index = 0;
        int end = pos;
        if (end < 0)
        {
            end = 0;
        }
        if (end > str.Length)
        {
            end = str.Length;
        }
        while (index < end)
        {
            char ch = str[index];
            if (ch == '\n')
            {
                lineNumber++;
                nearby.Length = 0;
                pointer.Length = 0;
            }
            else
            {
                pointer.Append(' ');
                nearby.Append(ch);
            }
            index++;
        }
        pointer.Append("^");
        if (index < str.Length)
        {
            while (index < str.Length)
            {
                char ch = str[index];
                if (ch == '\n' || ch == '\r')
                {
                    break;
                }
                nearby.Append(ch);
                index++;
            }
        }
        nearbyText = nearby.ToString();
        pointerText = pointer.ToString();
        return lineNumber;
    }
    public FileUtils() { }
}

partial class FileUtils
{
    private static void InitCharMappings()
    {
        _ = s_CharMappings.Length;
        for (int i = 0; i < s_CharMappings.Length; i += 2)
        {
            char ascii = s_CharMappings[i];
            char unicode = s_CharMappings[i + 1];
            s_UnicodeToAsciiTable[unicode] = ascii;
            s_AsciiToUnicodeTable[ascii] = unicode;
        }
    }
    private static void InitNamedCharMap()
    {
        _ = s_NameMappings.Length;
        for (int i = 0; i < s_NameMappings.Length; i += 4)
        {
            string shortcutKey = s_NameMappings[i];
            string longShortcutKey = s_NameMappings[i + 1];
            string unicodeString = s_NameMappings[i + 2];
            string charName = s_NameMappings[i + 3];

            if (charName != null)
            {
                s_CharNameToUnicodeStringTable[charName] = unicodeString;
            }

            if (unicodeString != null && unicodeString.Length == 1)
            {
                s_UnicodeToCharNameTable[unicodeString[0]] = charName;
            }

            if (longShortcutKey != null)
            {
                if (longShortcutKey.Length != 1)
                {
                    throw new Exception(String.Format("Greek Name Table init failed for char \"{0}\": \"{1}\".Length != 1", charName, longShortcutKey));
                }
                char longShortcutChar = longShortcutKey[0];
                if (unicodeString != null)
                {
                    s_LongUnicodeToShortcutKeyTable.Add(new Pair<string, char>(unicodeString, longShortcutChar));
                }
                else
                {
                    throw new ArgumentNullException("unicode_string", "Input shouldn't be null.");
                }
            }

            if (shortcutKey != null)
            {
                if (shortcutKey.Length != 1)
                {
                    throw new Exception("Greek Name Table init failed(2).");
                }
                char shortcutChar = shortcutKey[0];
                if (unicodeString != null && unicodeString.Length != 1)
                {
                    throw new Exception(String.Format("Greek Name Table init failed for char \"{0}\": \"{1}\".Length != 1", charName, unicodeString));
                }
                if (unicodeString == null)
                {
                    throw new ArgumentNullException("unicode_string", "Input shouldn't be null.");
                }
                char unicodeChar = unicodeString[0];
                s_GreekShortcutKeyToUnicodeTable[shortcutChar] = unicodeChar;
                s_UnicodeToGreekShortcutKeyTable[unicodeChar] = shortcutChar;
            }
        }
        s_bNamedCharMapInitialized = true;
    }
}
