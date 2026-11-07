#nullable disable
#pragma warning disable

namespace System.Reflection.Emit.Quire.Mpl;

partial class TypeNameBuilder
{
    private global::System.Text.StringBuilder _str = new global::System.Text.StringBuilder();
    private int _instNesting;
    private bool _firstInstArg;
    private bool _nestedName;
    private bool _hasAssemblySpec;
    private bool _useAngleBracketsForGenerics;
    private global::System.Collections.Generic.List<int> _stack = new global::System.Collections.Generic.List<int>();
    private int _stackIdx;
    private TypeNameBuilder() { }
    public override string ToString() => _str.ToString();
}
