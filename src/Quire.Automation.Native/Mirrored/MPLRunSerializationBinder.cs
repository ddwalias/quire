namespace Quire.Automation;

internal sealed partial class MPLRunSerializationBinder
{
    public override global::System.Type BindToType(string assemblyName, string typeName)
    {
        global::System.Type result = null;
        string fullName = typeof(global::Quire.Mcd.MatVal).Assembly.FullName;
        if (assemblyName.Equals(fullName))
        {
            result = typeof(global::Quire.Mcd.MatVal).Assembly.GetType(typeName);
        }

        return result;
    }

    public MPLRunSerializationBinder()
    {
    }
}
