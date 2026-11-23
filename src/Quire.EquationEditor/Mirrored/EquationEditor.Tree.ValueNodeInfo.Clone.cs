using Quire.Mirror;

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

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

    partial class b
    {
        [MirrorVerified("Release IL")]
        public b() { }
    }
}
