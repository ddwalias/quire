using System.Runtime.InteropServices;
using System.Text;

namespace Quire.utl_mgd;

public partial class LabelSet
{
    public LabelSet(SortedStringSet labels, [MarshalAs(UnmanagedType.U1)] bool wild)
    {
        m_labels = labels;
        m_wild = wild;
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public bool Unifies(LabelSet other)
    {
        if (this == other)
        {
            return true;
        }

        if (m_wild)
        {
            if (other.m_wild)
            {
                return true;
            }

            bool result = m_labels.IsSubsetOf(other.m_labels);
            GC.KeepAlive(this);
            return result;
        }

        if (other.m_wild)
        {
            bool result2 = other.m_labels.IsSubsetOf(m_labels);
            GC.KeepAlive(this);
            return result2;
        }

        bool result3 = m_labels.SetEquals(other.m_labels);
        GC.KeepAlive(this);
        return result3;
    }

    [return: MarshalAs(UnmanagedType.U1)]
    public static bool _unifies(LabelSet a, LabelSet b) => a.Unifies(b);

    public SortedStringSet Labels() => m_labels;

    [return: MarshalAs(UnmanagedType.U1)]
    public bool Wild() => m_wild;

    [return: MarshalAs(UnmanagedType.U1)]
    public bool Empty()
    {
        int num = ((m_labels.Count == 0 && !m_wild) ? 1 : 0);
        return (byte)num != 0;
    }

    public override string ToString()
    {
        if (m_cached_name == null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('{');
            EmitLabels(stringBuilder, ',');
            stringBuilder.Append('}');
            m_cached_name = stringBuilder.ToString();
        }

        GC.KeepAlive(this);
        return m_cached_name;
    }

    public void EmitLabels(StringBuilder b, [MarshalAs(UnmanagedType.U2)] char delimiter)
    {
        bool flag = true;
        foreach (string label in m_labels)
        {
            if (!flag)
            {
                b.Append(delimiter);
            }
            else
            {
                flag = false;
            }

            b.Append(label);
        }

        if (m_wild)
        {
            if (!flag)
            {
                b.Append(delimiter);
            }

            b.Append('*');
        }

        GC.KeepAlive(this);
    }

    public string[] ToVector()
    {
        string[] array = new string[m_labels.Count];
        m_labels.CopyTo(array);
        GC.KeepAlive(this);
        return array;
    }
}
