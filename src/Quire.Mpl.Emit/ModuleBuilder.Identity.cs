namespace System.Reflection.Emit.Quire.Mpl;

public partial class ModuleBuilder
{
    internal ModuleBuilder(string name, AssemblyBuilder assemblyBuilder)
    {
        ScopeName = name;
        Assembly = assemblyBuilder;
    }
}
