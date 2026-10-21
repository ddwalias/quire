using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Quire.utl_mgd;

public partial class LabelTrie
{
    public SortedStringSet m_labels;
    public LabelSet m_wild;
    public LabelSet m_non_wild;
    public Dictionary<string, LabelTrie> m_map = new Dictionary<string, LabelTrie>();
    public static LabelTrie s_root = new LabelTrie(new SortedStringSet());
    public static SafeReadWriteLock s_lock = new SafeReadWriteLock();
    private static char[] s_braces = "{}".ToCharArray();
    private static char[] s_comma = ",".ToCharArray();

    public LabelTrie(SortedStringSet labels)
    {
        m_labels = labels;
        m_wild = new LabelSet(labels, wild: true);
        m_non_wild = new LabelSet(labels, wild: false);
    }

    public static LabelSet Find(SortedStringSet labels, [MarshalAs(UnmanagedType.U1)] bool wild)
    {
        LabelTrie labelTrie = null;
        InterruptControl.ProhibitInterruption();
        s_lock.WaitReadLock();
        try
        {
            LabelTrie labelTrie2 = s_root;
            IEnumerator enumerator = labels.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string text = (string)enumerator.Current;
                    labelTrie = null;
                    if (!labelTrie2.m_map.TryGetValue(text, out labelTrie))
                    {
                        s_lock.ReleaseReadLock();
                        s_lock.WaitWriteLock();
                        try
                        {
                            if (!labelTrie2.m_map.TryGetValue(text, out labelTrie))
                            {
                                SortedStringSet sortedStringSet = new SortedStringSet(labelTrie2.m_labels);
                                sortedStringSet.Add(text);
                                labelTrie = new LabelTrie(sortedStringSet);
                                labelTrie2.m_map[text] = labelTrie;
                            }
                        }
                        finally
                        {
                            s_lock.ReleaseWriteLock();
                            s_lock.WaitReadLock();
                        }
                    }

                    labelTrie2 = labelTrie;
                }
            }
            finally
            {
                IEnumerator enumerator2 = enumerator;
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            return (!wild) ? labelTrie2.m_non_wild : labelTrie2.m_wild;
        }
        finally
        {
            s_lock.ReleaseReadLock();
            InterruptControl.AllowInterruption();
        }
    }

    public static LabelSet InternLabels(string s)
    {
        bool wild = false;
        SortedStringSet sortedStringSet = new SortedStringSet();
        int num = s.IndexOf('{');
        if (num == -1)
        {
            return s_root.m_non_wild;
        }

        s = s.Substring(num + 1);
        s = s.Trim(s_braces);
        string[] array = s.Split(s_comma);
        int num2 = array.Length;
        int num3 = 0;
        if (0 < num2)
        {
            do
            {
                string text = array[num3].Trim();
                array[num3] = text;
                if (array[num3].Length != 0)
                {
                    if (array[num3].Equals("*"))
                    {
                        wild = true;
                    }
                    else
                    {
                        sortedStringSet.Add(array[num3]);
                    }
                }

                num3++;
            }
            while (num3 < num2);
        }

        return Find(sortedStringSet, wild);
    }

    public static LabelSet InternLabels(string[] labels, [MarshalAs(UnmanagedType.U1)] bool wild) => Find(new SortedStringSet(labels), wild);

    public static LabelSet Empty() => s_root.m_non_wild;

    public static LabelSet WithWild(LabelSet s, [MarshalAs(UnmanagedType.U1)] bool bWild)
    {
        if (s.Wild() == bWild)
        {
            return s;
        }

        return Find(s.Labels(), bWild);
    }
}
