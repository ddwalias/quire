using System.Collections;

namespace Quire.utl_mgd;

public partial struct LabelUtils
{
    public static char[] s_sharp = "#".ToCharArray();

    public static string SplitName(string s)
    {
        int num = s.IndexOf('{');
        return (num != -1) ? s.Substring(0, num) : s;
    }

    public static LabelSet SplitLabels(string s) => LabelTrie.InternLabels(s);

    public static string SplitMangledName(string s)
    {
        int num = s.IndexOf('#');
        return (num != -1) ? s.Substring(0, num) : s;
    }

    public static LabelSet SplitMangledLabels(string s)
    {
        bool wild = false;
        int num = s.IndexOf('#');
        if (num == -1)
        {
            return LabelTrie.Empty();
        }

        s = s.Substring(num + 1);
        string[] array = s.Split(s_sharp);
        int num2 = array.Length;
        int num3 = num2 - 1;
        if (array[num3].Equals("wild"))
        {
            wild = true;
            num3 = num2 - 1;
            string[] array2 = new string[num3];
            int num4 = 0;
            num3 = num2 - 1;
            if (0 < num3)
            {
                do
                {
                    array2[num4] = array[num4];
                    num4++;
                }
                while (num4 < num3);
            }

            array = array2;
        }

        return LabelTrie.InternLabels(array, wild);
    }

    public static string Unmangle(string mangled)
    {
        string text = SplitMangledLabels(mangled).ToString();
        int num = mangled.IndexOf('#');
        return ((num != -1) ? mangled.Substring(0, num) : mangled) + text;
    }

    public static string Canonicalize(string s, ref bool wild)
    {
        LabelSet labelSet = LabelTrie.InternLabels(s);
        wild = labelSet.Wild();
        string text = labelSet.ToString();
        int num = s.IndexOf('{');
        return ((num != -1) ? s.Substring(0, num) : s) + text;
    }

    public static string Canonicalize(string s)
    {
        string text = LabelTrie.InternLabels(s).ToString();
        int num = s.IndexOf('{');
        return ((num != -1) ? s.Substring(0, num) : s) + text;
    }

    public static string ForceWild(string s)
    {
        LabelSet s2 = LabelTrie.InternLabels(s);
        s2 = LabelTrie.WithWild(s2, bWild: true);
        string text = s2.ToString();
        int num = s.IndexOf('{');
        return ((num != -1) ? s.Substring(0, num) : s) + text;
    }

    public static bool Unifies(string c, string c_non_wild)
    {
        int num = c.IndexOf('{');
        string obj = ((num != -1) ? c.Substring(0, num) : c);
        int num2 = c_non_wild.IndexOf('{');
        string value = ((num2 != -1) ? c_non_wild.Substring(0, num2) : c_non_wild);
        if (!obj.Equals(value))
        {
            return false;
        }

        LabelSet labelSet = LabelTrie.InternLabels(c);
        LabelSet other = LabelTrie.InternLabels(c_non_wild);
        return labelSet.Unifies(other);
    }

    public static object LookupPossiblyWild(string c, IDictionary d)
    {
        LabelSet labelSet = LabelTrie.InternLabels(c);
        if (!labelSet.Wild())
        {
            return d[c];
        }

        int num = c.IndexOf('{');
        string text = ((num != -1) ? c.Substring(0, num) : c);
        foreach (object item in d)
        {
            DictionaryEntry dictionaryEntry = (DictionaryEntry)item;
            string s = dictionaryEntry.Key as string;
            string value = SplitName(s);
            if (text.Equals(value))
            {
                LabelSet labelSet2 = LabelTrie.InternLabels(s);
                LabelSet labelSet3 = labelSet2;
                if (labelSet.Unifies(labelSet2))
                {
                    return dictionaryEntry.Value;
                }
            }
        }

        return null;
    }

    public static string FindBinding(string s, IDictionary d)
    {
        int num = s.IndexOf('{');
        string text = ((num != -1) ? s.Substring(0, num) : s);
        LabelSet labelSet = LabelTrie.InternLabels(s);
        foreach (object item in d)
        {
            DictionaryEntry dictionaryEntry = (DictionaryEntry)item;
            string text2 = dictionaryEntry.Key as string;
            string value = SplitName(text2);
            if (text.Equals(value))
            {
                LabelSet labelSet2 = LabelTrie.InternLabels(text2);
                LabelSet labelSet3 = labelSet2;
                if (labelSet.Unifies(labelSet2))
                {
                    return text2;
                }
            }
        }

        return null;
    }
}
