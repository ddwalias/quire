namespace Quire.EquationEditor.Tree.Parsing;

public partial struct Rule
{
    public readonly System.Collections.Generic.IList<string> Names;
    public readonly System.Collections.Generic.IList<int> Slots;

    public bool ApplyToAllSlots => Slots == null || Slots.Count == 0;

    public Rule(params string[] names) : this(names, null)
    {
    }

    public Rule(string name, params int[] slots) : this(new[] { name }, slots)
    {
    }

    public Rule(int slot, params string[] names) : this(names, new[] { slot })
    {
    }

    public Rule(string[] names, int[] slots)
    {
        Names = names;
        Slots = slots;
    }
}
