#nullable disable
#pragma warning disable

namespace System.Reflection.Emit.Quire.Mpl;

partial class TypeNameBuilder
{
    private global::System.Text.StringBuilder _str = new global::System.Text.StringBuilder();
    private global::System.Collections.Generic.List<int> _stack = new global::System.Collections.Generic.List<int>();
    private TypeNameBuilder() { }
    public override string ToString() => _str.ToString();
}
