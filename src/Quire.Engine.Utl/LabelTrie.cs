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
            LabelTrie currentTrie = s_root;
            IEnumerator enumerator = labels.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string text = (string)enumerator.Current;
                    labelTrie = null;
                    if (!currentTrie.m_map.TryGetValue(text, out labelTrie))
                    {
                        s_lock.ReleaseReadLock();
                        s_lock.WaitWriteLock();
                        try
                        {
                            if (!currentTrie.m_map.TryGetValue(text, out labelTrie))
                            {
                                SortedStringSet sortedStringSet = new SortedStringSet(currentTrie.m_labels);
                                sortedStringSet.Add(text);
                                labelTrie = new LabelTrie(sortedStringSet);
                                currentTrie.m_map[text] = labelTrie;
                            }
                        }
                        finally
                        {
                            s_lock.ReleaseWriteLock();
                            s_lock.WaitReadLock();
                        }
                    }

                    currentTrie = labelTrie;
                }
            }
            finally
            {
                IEnumerator currentEnumerator = enumerator;
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            return (!wild) ? currentTrie.m_non_wild : currentTrie.m_wild;
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
        int labelCount = array.Length;
        int labelIndex = 0;
        if (0 < labelCount)
        {
            do
            {
                string label = array[labelIndex].Trim();
                array[labelIndex] = label;
                if (array[labelIndex].Length != 0)
                {
                    if (array[labelIndex].Equals("*"))
                    {
                        wild = true;
                    }
                    else
                    {
                        sortedStringSet.Add(array[labelIndex]);
                    }
                }

                labelIndex++;
            }
            while (labelIndex < labelCount);
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
