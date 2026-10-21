using System.Collections;
using System.Runtime.InteropServices;

namespace Quire.utl_mgd;

public partial class LabeledDictionary
{
    public virtual object SyncRoot
    {
        get
        {
            object syncRoot = m_table.SyncRoot;
            GC.KeepAlive(this);
            return syncRoot;
        }
    }

    public virtual bool IsSynchronized
    {
        [return: MarshalAs(UnmanagedType.U1)]
        get
        {
            bool isSynchronized = m_table.IsSynchronized;
            GC.KeepAlive(this);
            return isSynchronized;
        }
    }

    public virtual int Count
    {
        get
        {
            int count = m_table.Count;
            GC.KeepAlive(this);
            return count;
        }
    }

    public virtual ICollection Values
    {
        get
        {
            ICollection values = m_table.Values;
            GC.KeepAlive(this);
            return values;
        }
    }

    public virtual ICollection Keys
    {
        get
        {
            ICollection keys = m_table.Keys;
            GC.KeepAlive(this);
            return keys;
        }
    }

    public virtual object this[object key]
    {
        get
        {
            bool wild = false;
            string text = LabelUtils.Canonicalize(key.ToString(), ref wild);
            if (!wild)
            {
                object result = m_table[text];
                GC.KeepAlive(this);
                return result;
            }

            object result2 = LabelUtils.LookupPossiblyWild(text, m_table);
            GC.KeepAlive(this);
            return result2;
        }
        set
        {
            OnChange();
            Hashtable table = m_table;
            string key2 = LabelUtils.Canonicalize(key.ToString());
            table[key2] = value;
            GC.KeepAlive(this);
        }
    }

    public virtual bool IsReadOnly
    {
        [return: MarshalAs(UnmanagedType.U1)]
        get => m_locked;
    }

    public virtual bool IsFixedSize
    {
        [return: MarshalAs(UnmanagedType.U1)]
        get => false;
    }

    public LabeledDictionary(IDictionary d)
    {
        m_table = new Hashtable(d.Count);
        m_locked = false;
        foreach (object item in d)
        {
            DictionaryEntry dictionaryEntry = (DictionaryEntry)item;
            object value = dictionaryEntry.Value;
            object key = dictionaryEntry.Key;
            this[key] = value;
        }

        GC.KeepAlive(this);
    }

    public LabeledDictionary(int n)
    {
        m_table = new Hashtable(n);
        m_locked = false;
    }

    public LabeledDictionary()
    {
        m_table = new Hashtable();
        m_locked = false;
    }

    public virtual LabeledDictionary Lock()
    {
        m_locked = true;
        return this;
    }

    public virtual void OnChange()
    {
    }

    public virtual void Add(object key, object value)
    {
        OnChange();
        Hashtable table = m_table;
        string key2 = LabelUtils.Canonicalize(key.ToString());
        table.Add(key2, value);
        GC.KeepAlive(this);
    }

    public virtual void Clear()
    {
        m_table.Clear();
        GC.KeepAlive(this);
    }

    public virtual bool Contains(object key)
    {
        bool wild = false;
        string text = LabelUtils.Canonicalize(key.ToString(), ref wild);
        if (!wild)
        {
            bool result = m_table.Contains(text);
            GC.KeepAlive(this);
            return result;
        }

        bool result2 = LabelUtils.LookupPossiblyWild(text, m_table) != null;
        GC.KeepAlive(this);
        return result2;
    }

    public int Contains1(string s) => Contains(s) ? 1 : 0;

    public virtual void Remove(object key)
    {
        OnChange();
        Hashtable table = m_table;
        string key2 = LabelUtils.Canonicalize(key.ToString());
        table.Remove(key2);
        GC.KeepAlive(this);
    }

    public virtual IDictionaryEnumerator _e1()
    {
        IDictionaryEnumerator enumerator = m_table.GetEnumerator();
        GC.KeepAlive(this);
        return enumerator;
    }

    IDictionaryEnumerator IDictionary.GetEnumerator() => _e1();

    public virtual IEnumerator _e2() => _e1();

    IEnumerator IEnumerable.GetEnumerator() => _e2();

    public virtual void CopyTo(Array array, int index)
    {
        m_table.CopyTo(array, index);
        GC.KeepAlive(this);
    }

    public LabeledDictionary Copy() => new LabeledDictionary(this);

    public LabeledDictionary MergeResult(LabeledDictionary d)
    {
        if (Contains("%errors"))
        {
            return this;
        }

        if (d.Contains("%errors"))
        {
            GC.KeepAlive(this);
            return d;
        }

        LabeledDictionary labeledDictionary = new LabeledDictionary(this);
        object value = d["%result"];
        labeledDictionary["%result"] = value;
        GC.KeepAlive(this);
        return labeledDictionary;
    }
}
