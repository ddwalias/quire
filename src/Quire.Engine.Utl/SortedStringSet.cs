using System.Collections;

namespace Quire.utl_mgd;

public partial class SortedStringSet
{
    public int Count
    {
        get
        {
            int count = m_strings.Count;
            GC.KeepAlive(this);
            return count;
        }
    }

    public SortedStringSet(string[] strings)
    {
        m_strings = new SortedList();
        int num = 0;
        if (0 < strings.LongLength)
        {
            do
            {
                m_strings.Add(strings[num], true);
                num++;
            }
            while (num < strings.LongLength);
        }

        GC.KeepAlive(this);
    }

    public SortedStringSet(SortedStringSet other) => m_strings = new SortedList(other.m_strings);

    public SortedStringSet() => m_strings = new SortedList();

    public virtual IEnumerator GetEnumerator()
    {
        IEnumerator enumerator = m_strings.Keys.GetEnumerator();
        GC.KeepAlive(this);
        return enumerator;
    }

    public bool IsSubsetOf(SortedStringSet other)
    {
        foreach (string key in m_strings.Keys)
        {
            if (!other.m_strings.Contains(key))
            {
                GC.KeepAlive(this);
                return false;
            }
        }

        GC.KeepAlive(this);
        return true;
    }

    public bool SetEquals(SortedStringSet other)
    {
        int count = m_strings.Count;
        GC.KeepAlive(this);
        int count2 = other.m_strings.Count;
        GC.KeepAlive(other);
        if (count != count2)
        {
            GC.KeepAlive(this);
            return false;
        }

        return IsSubsetOf(other);
    }

    public void CopyTo(string[] a)
    {
        m_strings.Keys.CopyTo(a, 0);
        GC.KeepAlive(this);
    }

    public void Add(string s)
    {
        m_strings.Add(s, true);
        GC.KeepAlive(this);
    }

    public bool Contains(string s)
    {
        bool result = m_strings.Contains(s);
        GC.KeepAlive(this);
        return result;
    }
}
