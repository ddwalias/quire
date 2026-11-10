namespace Quire.EquationEditor.Tree;

public abstract partial class ValueNodeInfo
{
    public object Clone()
    {
        return bk();
    }
}
