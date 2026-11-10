namespace Quire.EquationEditor.Tree;

public abstract partial class ValueNodeInfo
{
    public virtual bool IsEquivalentTo(INodeInfo other)
    {
        if (other is not ValueNodeInfo valueNodeInfo)
        {
            return false;
        }

        return Value.Equals(valueNodeInfo.Value);
    }

    public virtual void MakeReadOnly()
    {
        if (Value is global::Quire.EquationEditor.Tree.IReadOnlyAbstract readOnly)
        {
            readOnly.MakeReadOnly();
        }
    }

    public virtual void MakeEditable()
    {
        if (Value is global::Quire.EquationEditor.Tree.IReadOnlyAbstract readOnly)
        {
            readOnly.MakeEditable();
        }
    }
}
