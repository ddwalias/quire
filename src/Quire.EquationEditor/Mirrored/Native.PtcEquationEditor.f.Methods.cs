namespace Quire.Native.PtcEquationEditor;

internal partial class f
{
    private readonly global::System.Collections.Generic.Dictionary<char, char> b = new global::System.Collections.Generic.Dictionary<char, char>();

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    private f()
    {
        a('A', 'Α');
        a('B', 'Β');
        a('C', 'Χ');
        a('D', 'Δ');
        a('E', 'Ε');
        a('H', 'Η');
        a('G', 'Γ');
        a('I', 'Ι');
        a('K', 'Κ');
        a('L', 'Λ');
        a('M', 'Μ');
        a('N', 'Ν');
        a('W', 'Ω');
        a('O', 'Ο');
        a('F', 'Φ');
        a('P', 'Π');
        a('Y', 'Ψ');
        a('R', 'Ρ');
        a('S', 'Σ');
        a('T', 'Τ');
        a('Q', 'Θ');
        a('J', 'ϑ');
        a('U', 'Υ');
        a('X', 'Ξ');
        a('Z', 'Ζ');
        a('a', 'α');
        a('b', 'β');
        a('c', 'χ');
        a('d', 'δ');
        a('e', 'ε');
        a('h', 'η');
        a('g', 'γ');
        a('i', 'ι');
        a('k', 'κ');
        a('l', 'λ');
        a('m', 'μ');
        a('n', 'ν');
        a('w', 'ω');
        a('o', 'ο');
        a('f', 'ϕ');
        a('j', 'φ');
        a('p', 'π');
        a('y', 'ψ');
        a('r', 'ρ');
        a('s', 'σ');
        a('t', 'τ');
        a('q', 'θ');
        a('u', 'υ');
        a('x', 'ξ');
        a('z', 'ζ');
    }

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
