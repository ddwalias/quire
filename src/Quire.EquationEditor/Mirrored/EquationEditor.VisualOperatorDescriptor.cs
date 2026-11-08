namespace Quire.EquationEditor;

public partial class VisualOperatorDescriptor
{
    // Obfuscated original field names: the Release IL keeps them.
    private string a;
    private string b;
    private string c;
    private VisualOperatorMenuStyle d;
    private string e;

    public string CanonicalName
    {
        get => a;
        set => a = value;
    }

    public string Name
    {
        get => b;
        set => b = value;
    }

    public string Description
    {
        get => c;
        set => c = value;
    }

    public VisualOperatorMenuStyle MenuStyle
    {
        get => d;
        set => d = value;
    }

    public string VisualRepresentation
    {
        get => e;
        set => e = value;
    }

    public VisualOperatorDescriptor()
    {
    }
}
