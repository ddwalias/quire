namespace Quire.EquationEditor.Tree;

public abstract partial class ValueNodeInfo
{
    protected ValueNodeInfo()
    {
    }

    protected ValueNodeInfo(ValueNodeInfo other)
    {
        if (other.Value is global::System.ICloneable cloneable)
        {
            Value = cloneable.Clone();
        }
        else
        {
            Value = other.Value;
        }
    }

    public object Clone()
    {
        return bk();
    }
}
