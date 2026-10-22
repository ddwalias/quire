#nullable disable
#pragma warning disable

namespace System.Reflection.Emit.Quire.Mpl;

partial class AssemblyBuilder
{
    private bool _previouslySaved;
    private global::System.Reflection.AssemblyName _assemblyName;

    public void LoadAssembly()
    {
        if (!global::System.Linq.Enumerable.Any(global::System.AppDomain.CurrentDomain.GetAssemblies(), (global::System.Reflection.Assembly a) => (a.GetName()?.Name?.Equals(Name, global::System.StringComparison.OrdinalIgnoreCase)).GetValueOrDefault()))
        {
            global::System.Reflection.Assembly.Load(EmitAssembly());
        }
    }

    private global::System.Reflection.Emit.Quire.Mpl.ModuleBuilder _module;
    private string _fullName;
    private global::System.Version _version;
    private string _culture;
    internal global::System.Reflection.Metadata.AssemblyDefinitionHandle _assemblyDef;
}
