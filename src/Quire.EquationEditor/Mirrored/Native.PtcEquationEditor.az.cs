using System.Collections.Generic;
using Quire.EquationEditor.Tree.Parsing;

namespace Quire.Native.PtcEquationEditor;

internal partial class az : List<Rule>
{
    public az()
    {
    }

    public az(IEnumerable<Rule> A_0)
        : base(A_0)
    {
    }

    public bool c(string A_0, int A_1)
    {
        return a(A_0, A_1).HasValue;
    }

    public bool a(int A_0)
    {
        foreach (Rule current in this)
        {
            if (current.ApplyToAllSlots)
            {
                return true;
            }

            if (current.Slots.Contains(A_0))
            {
                return true;
            }
        }

        return false;
    }

    public IList<Rule> b(string A_0, int A_1)
    {
        List<Rule> list = new List<Rule>();
        foreach (Rule current in this)
        {
            if (!current.Names.Contains(A_0) && (current.ApplyToAllSlots || current.Slots.Contains(A_1)))
            {
                list.Add(current);
            }
        }

        return list;
    }

    public Rule? a(string A_0, int A_1)
    {
        foreach (Rule current in this)
        {
            if (current.Names.Contains(A_0) && (current.ApplyToAllSlots || current.Slots.Contains(A_1)))
            {
                return current;
            }
        }

        return null;
    }
}
