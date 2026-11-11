namespace Quire.Native.PtcEquationEditor;

internal partial class f
{
    public char a(char value)
    {
        if (b.ContainsKey(value))
        {
            return b[value];
        }

        return value;
    }

    private void a(char first, char second)
    {
        b.Add(first, second);
        b.Add(second, first);
    }
}
