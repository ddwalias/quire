#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Quire.Engine.Misc;

partial class XmlUtils
{
    private static bool s_Debug = false;
    private static string s_WhitepaceChars = " \t\f\n\r";
    private static string s_TagBreakChars = " \t\f\n\r\"\'/>";
    private static char s_OpenAngle = '<';
    private static char s_CloseAngle = '>';
    private static char s_DoubleQuote = '"';
    private static char s_SingleQuote = '\'';
    private static char s_Slash = '/';
    private static char s_Excl = '!';
    private static char s_Hyphen = '-';
    private static char s_CloseBracket = ']';
    private static char s_EqualSign = '=';
    private static string s_xmlns = "xmlns";
    protected static bool s_TightenXmlnsAttributes = true;
    private static int s_TestsDone = 0;
    private static int s_TestsPassed = 0;
    private static int s_TestNum = 0;
    private static int s_TestPos = 0;
    public static bool TightenXmlnsAttributes
    {
        get => s_TightenXmlnsAttributes;
        set => s_TightenXmlnsAttributes = value;
    }
    public static void BadSyntax(string str, int pos, string msg)
    {
        throw new Exception(string.Format("bad_syntax\t{0}\tpos={1}\ttext={2}", msg, pos, str));
    }
    private static void BadSyntax(string str, int pos, string fmt, object first)
    {
        BadSyntax(str, pos, string.Format(fmt, first));
    }
    private static void BadSyntax(string str, int pos, string fmt, object first, object second)
    {
        BadSyntax(str, pos, string.Format(fmt, first, second));
    }
    private static void BadSyntax(string str, int pos, string fmt, object first, object second, object third)
    {
        BadSyntax(str, pos, string.Format(fmt, first, second, third));
    }
    private static void Dbg(string str)
    {
        if (s_Debug)
        {
            Console.WriteLine(str);
        }
    }
    private static void Dbg(string fmt, object first)
    {
        Dbg(string.Format(fmt, first));
    }
    private static void Dbg(string fmt, object first, object second)
    {
        Dbg(string.Format(fmt, first, second));
    }
    private static void Dbg(string fmt, object first, object second, object third)
    {
        Dbg(string.Format(fmt, first, second, third));
    }
    public static bool LookingAtText(string str, int scanPos, string text)
    {
        int num = scanPos;
        foreach (char obj in text)
        {
            if (num >= text.Length)
            {
                return false;
            }
            if (!str[num].Equals(obj))
            {
                return false;
            }
            num++;
        }
        return true;
    }
    public static bool RequireText(string str, ref int scanPos, string text)
    {
        if (!LookingAtText(str, scanPos, text))
        {
            Dbg("RequireText \"{0}\" failing @ {1} in {2}", text, scanPos, str);
            return false;
        }
        scanPos += text.Length;
        return true;
    }
    public static bool FindElementStart(string str, ref int startPos)
    {
        return FindElementStart(str, ref startPos, s_WhitepaceChars);
    }
    public static bool FindElementStart(string str, ref int startPos, string whitespace)
    {
        for (int i = startPos; i < str.Length; i++)
        {
            char value = str[i];
            int num = whitespace.IndexOf(value);
            Dbg("ch=" + value + " wspos=" + num);
            if (num == -1)
            {
                num = s_WhitepaceChars.IndexOf(value);
                if (num == -1)
                {
                    startPos = i;
                    Dbg("findElementStart succeeded @ " + i);
                    return true;
                }
                startPos = i;
                Dbg("FindElementStart whitespace barrier @ " + i);
                return false;
            }
        }
        Dbg("FindElementStart failing");
        return false;
    }
    public static bool ScanElement(string str, ref int startPos, out int endPos)
    {
        int elementStart = startPos;
        endPos = startPos;
        if (!FindElementStart(str, ref elementStart))
        {
            return false;
        }
        int scanPos = elementStart;
        if (!SkipElement(str, ref scanPos))
        {
            return false;
        }
        startPos = elementStart;
        endPos = scanPos;
        return true;
    }
    public static bool SkipElement(string str, ref int scanPos)
    {
        if (!FindElementStart(str, ref scanPos))
        {
            return false;
        }
        char c = str[scanPos];
        if (c.Equals(s_OpenAngle))
        {
            string elementName;
            bool isEmpty;
            bool isClosing;
            int startPos = scanPos;
            int endPos = startPos;
            if (ScanTag(str, ref startPos, out endPos, out elementName, out isEmpty, out isClosing))
            {
                startPos = endPos;
                if (!isEmpty)
                {
                    if (isClosing)
                    {
                        BadSyntax(str, startPos, "</{0}> seen out of context.", elementName);
                    }
                    if (!SkipMatchingTag(str, elementName, ref endPos))
                    {
                        BadSyntax(str, scanPos, "Unable to find matching tag.");
                    }
                }
            }
            else if (!SkipBracketedSGML(str, ref endPos))
            {
                BadSyntax(str, scanPos, "Unable to scan tag.");
            }
            scanPos = endPos;
            return true;
        }
        BadSyntax(str, scanPos, "Bad character where '<' expected.");
        return false;
    }
    public static bool ScanTag(string str, ref int startPos, out int endPos, out string elementName, out bool isEmpty, out bool isClosing)
    {
        bool sawSlash = false;
        bool closingTag = false;
        int tagStart = startPos;
        int cursor = tagStart;
        elementName = null;
        endPos = startPos;
        isEmpty = false;
        isClosing = false;
        if (cursor == str.Length || !str[cursor].Equals(s_OpenAngle))
        {
            BadSyntax(str, startPos, "Tag must start with '<'");
        }
        cursor++;
        if (cursor == str.Length)
        {
            BadSyntax(str, startPos, "Premature end of stream while reading tag.");
        }
        if (str[cursor].Equals(s_Slash))
        {
            cursor++;
            closingTag = true;
        }
        else if (str[cursor].Equals(s_Excl))
        {
            return false;
        }
        int i;
        if (!ScanTagName(str, ref cursor, out i))
        {
            BadSyntax(str, startPos, "Missing tag name.");
        }
        string text = str.Substring(cursor, i - cursor);
        endPos = startPos;
        for (; i < str.Length; i++)
        {
            char c = str[i];
            if (c.Equals(s_Slash))
            {
                sawSlash = true;
                continue;
            }
            if (c.Equals(s_CloseAngle))
            {
                if (text == null)
                {
                    BadSyntax(str, startPos, "Missing tag name.");
                }
                elementName = text;
                isEmpty = sawSlash;
                isClosing = closingTag;
                startPos = tagStart;
                endPos = i + 1;
                Dbg("Found tag {0} @ {1}", str.Substring(startPos, endPos - startPos), endPos);
                return true;
            }
            sawSlash = false;
            if (c.Equals(s_SingleQuote) || c.Equals(s_DoubleQuote))
            {
                if (!ScanAttributeString(str, ref i))
                {
                    BadSyntax(str, i, "Bad string literal.");
                }
                i--;
            }
        }
        BadSyntax(str, startPos, "Missing '>' in tag.");
        return false;
    }
    public static bool ScanTagName(string str, ref int startPos, out int endPos)
    {
        int i;
        for (i = startPos; i < str.Length; i++)
        {
            if (s_TagBreakChars.IndexOf(str[i]) < 0)
            {
                break;
            }
        }
        endPos = startPos;
        for (int j = i; j < str.Length; j++)
        {
            if (s_TagBreakChars.IndexOf(str[j]) >= 0)
            {
                if (j == startPos)
                {
                    BadSyntax(str, startPos, "Missing tag name.");
                }
                startPos = i;
                endPos = j;
                Dbg("ScanTagName win @ {0}", endPos);
                return true;
            }
        }
        BadSyntax(str, startPos, "No end to tag name.");
        return false;
    }
    public static bool ScanAttributeString(string str, ref int scanPos)
    {
        Dbg("Scanning attribute string @ {0}", scanPos);
        if (scanPos >= str.Length)
        {
            BadSyntax(str, scanPos, "Scan string at end of file.");
        }
        int num = scanPos;
        char obj = str[num];
        if (!obj.Equals(s_SingleQuote) && !obj.Equals(s_DoubleQuote))
        {
            BadSyntax(str, scanPos, "Trying to ScanAttributeString on non-String");
        }
        for (num++; num < str.Length; num++)
        {
            char c = str[num];
            if (c.Equals(s_CloseAngle))
            {
                BadSyntax(str, num, "Found '>' in attribute value. Should be &gt;.");
                return false;
            }
            if (c.Equals(obj))
            {
                scanPos = num + 1;
                Dbg("ScanAttributeString win @ {0}", num);
                return true;
            }
        }
        BadSyntax(str, scanPos, "String not closed.");
        return false;
    }
    public static bool SkipMatchingTag(string str, string elementName, ref int scanPos)
    {
        if (elementName == null || elementName.Length == 0)
        {
            throw new Exception("Matching null tag?");
        }
        int num = 1;
        for (int i = scanPos; i < str.Length; i++)
        {
            char c = str[i];
            if (!c.Equals(s_OpenAngle))
            {
                continue;
            }
            string foundName;
            int endPos;
            int startPos = i;
            bool isEmpty;
            bool isClosing;
            if (ScanTag(str, ref startPos, out endPos, out foundName, out isEmpty, out isClosing))
            {
                if (foundName != null && foundName.Equals(elementName) && !isEmpty)
                {
                    if (isClosing)
                    {
                        scanPos = endPos;
                        num--;
                        if (num < 1)
                        {
                            Dbg("Found " + elementName + " matching tag @ " + endPos.ToString());
                            return true;
                        }
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            else if (SkipCDATA(str, ref i))
            {
                i--;
            }
            else if (SkipSgmlComment(str, ref i))
            {
                i--;
            }
            else
            {
                BadSyntax(str, scanPos, "Stray <");
            }
        }
        BadSyntax(str, scanPos, "Can't find </{0}>", elementName);
        return false;
    }
    public static bool SkipBracketedSGML(string str, ref int scanPos)
    {
        int probePos = scanPos;
        if (SkipSgmlComment(str, ref probePos))
        {
            scanPos = probePos;
            return true;
        }
        if (SkipCDATA(str, ref probePos))
        {
            scanPos = probePos;
            return true;
        }
        return false;
    }
    public static bool SkipSgmlComment(string str, ref int scanPos)
    {
        int i = scanPos;
        Dbg("SgmlComment entered, pos={0}", i);
        RequireText(str, ref i, "<!");
        int num = 0;
        bool flag = false;
        for (; i < str.Length; i++)
        {
            char value = str[i];
            if (value.Equals(s_Hyphen))
            {
                num++;
                if (num == 2)
                {
                    num = 0;
                    flag = !flag;
                }
                continue;
            }
            num = 0;
            if (!flag)
            {
                if (value.Equals(s_CloseAngle))
                {
                    scanPos = i + 1;
                    Dbg("SgmlCommentEnd = {0}", scanPos);
                    return true;
                }
                if (s_WhitepaceChars.IndexOf(value) < 0)
                {
                    BadSyntax(str, i, "Strange state of SGML comment.");
                }
            }
        }
        return false;
    }
    private static bool FindCandidateXmlnsAttr(string str, ref int scanPos)
    {
        int searchPos = scanPos;
        while (true)
        {
            int xmlnsPos = str.IndexOf(s_xmlns, searchPos);
            int cursor = xmlnsPos;
            if (cursor < 0)
            {
                return false;
            }
            while (cursor > searchPos)
            {
                cursor--;
                if (str[cursor].Equals(s_CloseAngle))
                {
                    break;
                }
                if (str[cursor].Equals(s_OpenAngle))
                {
                    scanPos = cursor;
                    return true;
                }
            }
            searchPos = xmlnsPos + s_xmlns.Length;
        }
    }
    public static string StripXmlnsAttrs(string str)
    {
        if (str.IndexOf("xmlns") < 0)
        {
            return str;
        }
        StringBuilder stringBuilder = new StringBuilder();
        Queue<Triple<int, int, int>> skipAttrs;
        int num = 0;
        int scanPos = num;
        while (FindCandidateXmlnsAttr(str, ref scanPos))
        {
            stringBuilder.Append(str.Substring(num, scanPos - num));
            num = scanPos;
            if (!FindXmlnsAttrs(str, ref scanPos, out skipAttrs))
            {
                return str;
            }
            foreach (Triple<int, int, int> item in skipAttrs)
            {
                int first = item.First;
                _ = item.Second;
                int third = item.Third;
                stringBuilder.Append(str.Substring(num, first - num));
                num = third;
            }
            scanPos = num;
        }
        stringBuilder.Append(str.Substring(scanPos));
        return stringBuilder.ToString();
    }
    public static bool EquivalentXml(string leftXml, string rightXml)
    {
        string text = TightenXmlWhitespace(leftXml);
        string value = TightenXmlWhitespace(rightXml);
        return text.Equals(value);
    }
    public static string TightenXmlWhitespace(string rawstr)
    {
        string text = (s_TightenXmlnsAttributes ? StripXmlnsAttrs(rawstr) : rawstr);
        int i = 0;
        int length = text.Length;
        bool flag = false;
        StringBuilder stringBuilder = new StringBuilder();
        for (; i < length; i++)
        {
            char value = text[i];
            int num = i;
            int scanPos = i;
            if (value.Equals(s_OpenAngle))
            {
                flag = false;
                if (LookingAtText(text, i, "<!") && SkipBracketedSGML(text, ref scanPos))
                {
                    stringBuilder.Append(text, i, scanPos - i);
                    i = scanPos - 1;
                    continue;
                }
                num = i + 1;
                scanPos = text.IndexOf(s_CloseAngle, num + 1);
                if (scanPos < 0)
                {
                    return text;
                }
                if (text.IndexOf("xml:space=\"preserve\"", num, scanPos - num) >= 0 || text.IndexOf("xml:space='preserve'", num, scanPos - num) >= 0)
                {
                    num = i;
                    if (!ScanElement(text, ref num, out scanPos))
                    {
                        return text;
                    }
                    stringBuilder.Append(text, i, scanPos - i);
                    i = scanPos - 1;
                }
                else
                {
                    stringBuilder.Append(value);
                }
            }
            else if (value.Equals(s_CloseAngle))
            {
                flag = false;
                stringBuilder.Append(value);
                i++;
                SkipWhitespace(text, ref i);
                i--;
            }
            else if (s_WhitepaceChars.IndexOf(value) >= 0)
            {
                flag = true;
                i++;
                SkipWhitespace(text, ref i);
                i--;
            }
            else
            {
                if (flag)
                {
                    stringBuilder.Append(' ');
                }
                flag = false;
                stringBuilder.Append(value);
            }
        }
        return stringBuilder.ToString();
    }
    public static bool FindXmlnsAttrs(string str, ref int scanPos, out Queue<Triple<int, int, int>> skipAttrs)
    {
        int num = scanPos;
        int i;
        int attrNameStart = -1;
        int equalsPos = 0;
        string text;
        skipAttrs = new Queue<Triple<int, int, int>>();
        if (num == str.Length || !str[num].Equals(s_OpenAngle))
        {
            BadSyntax(str, scanPos, "Tag must start with '<'");
        }
        num++;
        if (num == str.Length)
        {
            BadSyntax(str, scanPos, "Premature end of stream while reading tag.");
        }
        if (str[num].Equals(s_Slash))
        {
            num++;
        }
        else if (str[num].Equals(s_Excl))
        {
            return false;
        }
        if (!ScanTagName(str, ref num, out i))
        {
            BadSyntax(str, scanPos, "Missing tag name.");
        }
        text = str.Substring(num, i - num);
        for (; i < str.Length; i++)
        {
            char value = str[i];
            if (value.Equals(s_Slash))
            {
                attrNameStart = -1;
                continue;
            }
            if (s_WhitepaceChars.IndexOf(value) >= 0)
            {
                attrNameStart = -1;
                continue;
            }
            if (value.Equals(s_EqualSign))
            {
                equalsPos = i;
                continue;
            }
            if (value.Equals(s_CloseAngle))
            {
                attrNameStart = -1;
                if (text == null)
                {
                    BadSyntax(str, scanPos, "Missing tag name.");
                }
                scanPos = i + 1;
                return true;
            }
            if (value.Equals(s_SingleQuote) || value.Equals(s_DoubleQuote))
            {
                if (!ScanAttributeString(str, ref i))
                {
                    BadSyntax(str, i, "Bad string literal.");
                }
                if (attrNameStart >= 0)
                {
                    string attrName = str.Substring(attrNameStart, equalsPos - attrNameStart);
                    if (attrName.Equals("xmlns") || attrName.StartsWith("xmlns:"))
                    {
                        skipAttrs.Enqueue(new Triple<int, int, int>(BackOverWhitespace(str, attrNameStart), attrNameStart, i));
                    }
                }
                i--;
            }
            else if (attrNameStart < 0)
            {
                attrNameStart = i;
            }
        }
        BadSyntax(str, scanPos, "Missing '>' in tag.");
        return false;
    }
    private static bool SkipWhitespace(string str, ref int pos)
    {
        int length = str.Length;
        while (pos < length)
        {
            char value = str[pos];
            if (s_WhitepaceChars.IndexOf(value) < 0)
            {
                break;
            }
            pos++;
        }
        return true;
    }
    private static int BackOverWhitespace(string str, int pos)
    {
        int num = pos;
        while (num > 0)
        {
            if (s_WhitepaceChars.IndexOf(str[num - 1]) < 0)
            {
                return num;
            }
            num--;
        }
        return num;
    }
    public static bool SkipCDATA(string str, ref int scanPos)
    {
        int i = scanPos;
        int num = 0;
        if (!RequireText(str, ref i, "<![CDATA["))
        {
            Dbg("CDATA failing @ {0} (no prefix), not updating scan pointer", i);
            return false;
        }
        for (; i < str.Length; i++)
        {
            char c = str[i];
            if (c.Equals(s_CloseBracket))
            {
                num++;
                Dbg("CDATA bracket count = {0} @ pos {1}", num, i);
                if (num > 2)
                {
                    num = 2;
                    Dbg("CDATA Resetting bracket count to 2 @ pos {0}", i);
                }
            }
            else
            {
                if (num == 2 && c.Equals(s_CloseAngle))
                {
                    i++;
                    scanPos = i;
                    Dbg("CDATA winning @ {0}, updating scan pointer", i);
                    return true;
                }
                num = 0;
            }
        }
        Dbg("CDATA failing @ {0}, not updating scan pointer", i);
        return false;
    }
    public static bool UnorderedEqual(string actual, string expected)
    {
        return UnorderedEqual(actual, expected, false);
    }
    public static bool UnorderedEqual(string leftXml, string rightXml, bool exact)
    {
        if (leftXml.Equals(rightXml))
        {
            return true;
        }
        int startPos = 0;
        int rightStart = 0;
        int endPos;
        int rightEnd;
        string elementName;
        string rightName;
        bool isEmpty;
        bool rightIsEmpty;
        bool isClosing;
        bool rightIsClosing;
        if (!ScanTag(leftXml, ref startPos, out endPos, out elementName, out isEmpty, out isClosing))
        {
            return false;
        }
        if (!ScanTag(rightXml, ref rightStart, out rightEnd, out rightName, out rightIsEmpty, out rightIsClosing))
        {
            return false;
        }
        if (isClosing || rightIsClosing)
        {
            return false;
        }
        if (elementName != rightName)
        {
            return false;
        }
        string leftOrdered = OrderedArbitrarily(leftXml);
        string rightOrdered = OrderedArbitrarily(rightXml);
        if (exact)
        {
            return leftOrdered == rightOrdered;
        }
        return EquivalentXml(leftOrdered, rightOrdered);
    }
    public static string OrderedArbitrarily(string xml)
    {
        int startPos = 0;
        int endPos;
        string elementName;
        bool isEmpty;
        bool isClosing;
        if (!ScanTag(xml, ref startPos, out endPos, out elementName, out isEmpty, out isClosing))
        {
            BadSyntax(xml, startPos, "Element opening tag is ill-formed.");
        }
        if (isEmpty)
        {
            return string.Format("{0}></{1}>", xml.Substring(startPos, endPos - 2));
        }
        if (isClosing)
        {
            BadSyntax(xml, startPos, "Element starts with close tag?");
        }
        int num = xml.LastIndexOf('<');
        if (num < endPos)
        {
            BadSyntax(xml, startPos, "Element in unexpected format.");
        }
        SortedList sortedList = new SortedList();
        int cursor = endPos;
        int elementEnd;
        while (cursor < num && ScanElement(xml, ref cursor, out elementEnd))
        {
            string text = xml.Substring(cursor, elementEnd - cursor);
            sortedList.Add(text, text);
            cursor = elementEnd;
        }
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xml, 0, endPos);
        foreach (string value in sortedList.GetValueList())
        {
            stringBuilder.Append(value);
        }
        stringBuilder.Append(xml, num, xml.Length - num);
        return stringBuilder.ToString();
    }
    public static void TestInit()
    {
        s_TestsDone = 0;
        s_TestsPassed = 0;
        s_TestNum = 0;
        s_TestPos = 0;
    }
    public static void TestCheck(string str, bool result, bool expectedResult, int expectedPos)
    {
        if (result != expectedResult)
        {
            Console.WriteLine("Test #{0} failed. res={1}, expected={2}, pos={3}", s_TestNum, result, expectedResult, s_TestPos);
        }
        else if (!expectedResult || s_TestPos != expectedPos)
        {
            Console.WriteLine("Test #{0} failed. pos={1}, expected={2}", s_TestNum, s_TestPos, expectedPos);
        }
        else
        {
            s_TestsPassed++;
        }
        BumpTest();
    }
    public static void TestCheck(bool b)
    {
        if (b)
        {
            s_TestsPassed++;
        }
        BumpTest();
    }
    public static void BumpTest()
    {
        s_TestsDone++;
        s_TestPos = 0;
        s_TestNum++;
    }
    public static void TestFinalize()
    {
        string text = (s_TestsPassed == s_TestsDone) ? "Passed." : "Failed.";
        Console.WriteLine("{0} {1}/{2} tests succeeded.", text, s_TestsPassed, s_TestsDone);
    }
    public static void SelfTest(string[] ignoredData)
    {
        TestInit();
        string str;
        TestCheck(str = "<!--foo-->a", SkipSgmlComment(str, ref s_TestPos), true, 10);
        TestCheck(str = "<!--foo>-->a", SkipSgmlComment(str, ref s_TestPos), true, 11);
        TestCheck(str = "<!--foo<-->a", SkipSgmlComment(str, ref s_TestPos), true, 11);
        TestCheck(str = "<![CDATA[foo]]>a", SkipCDATA(str, ref s_TestPos), true, 15);
        TestCheck(str = "<![CDATA[foo]>]]>a", SkipCDATA(str, ref s_TestPos), true, 17);
        TestCheck(str = "<![CDATA[foo]]]]>a", SkipCDATA(str, ref s_TestPos), true, 17);
        TestCheck(str = "<![CDATA[foo]>]]]>a", SkipCDATA(str, ref s_TestPos), true, 18);
        TestCheck(str = "<x>foo</x>a", SkipElement(str, ref s_TestPos), true, 10);
        TestCheck(str = "<x>foo</x>a", SkipElement(str, ref s_TestPos), true, 10);
        TestCheck(str = "<x>foo<y/></x>a", SkipElement(str, ref s_TestPos), true, 14);
        TestCheck(str = "<x>foo<y></x>a", SkipElement(str, ref s_TestPos), true, 13);
        TestCheck(str = "<x>foo<x/>bar</x>a", SkipElement(str, ref s_TestPos), true, 17);
        TestCheck(str = "<x>foo<x>bar</x>baz</x>a", SkipElement(str, ref s_TestPos), true, 23);
        TestCheck(str = "<x foo=\"xxx\"/>abc</x>", SkipElement(str, ref s_TestPos), true, 14);
        TestCheck(str = "<x foo='xxx'/>abc</x>", SkipElement(str, ref s_TestPos), true, 14);
        TestCheck(EquivalentXml("<foo x='3'   y='4' />", "<foo x='3' y='4'   />"));
        TestCheck(EquivalentXml("<foo  x='3'  >  <bar xml:space=\"preserve\">  abc </bar> </foo>", "<foo x='3'><bar xml:space=\"preserve\">  abc </bar></foo>"));
        TestCheck(TightenXmlWhitespace("<foo x='3'   y='4' />") == "<foo x='3' y='4' />");
        TestCheck(TightenXmlWhitespace("<foo  x='3'  >  <bar xml:space=\"preserve\">  abc </bar> </foo>") == "<foo x='3'><bar xml:space=\"preserve\">  abc </bar></foo>");
        TestCheck(TightenXmlWhitespace("<foo  x='3'  >  <bar >  abc\n   def   </bar> </foo>") == "<foo x='3'><bar>abc def</bar></foo>");
        TestFinalize();
        string text = "<x y=\"alpha\">a xmlns=\"gack\" or xmlns:zowie=\"grumble\" is cool.<q/></x>";
        string input = "<x xmlns=\"foo\" y=\"alpha\" xmlns:zip=\"xyz\">a xmlns=\"gack\" or xmlns:zowie=\"grumble\" is cool.<q xmlns:zing=\"hm\"/></x>";
        Console.WriteLine("Trying Xmlns tests...");
        string actual = StripXmlnsAttrs(input);
        if (actual.Equals(text))
        {
            Console.WriteLine("OK: {0} ==> {1}", input, actual);
        }
        else
        {
            Console.WriteLine("FAILED: {0} != {1}", actual, text);
        }
    }
    public XmlUtils() { }
}
