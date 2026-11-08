namespace Quire.EquationEditor.Tree;

public partial struct SymbolMargin
{
    public double Left;
    public double Right;

    public SymbolMargin(double margin)
    {
        Left = Right = margin;
    }

    public static implicit operator SymbolMargin(int margin) => new SymbolMargin(margin);
}
