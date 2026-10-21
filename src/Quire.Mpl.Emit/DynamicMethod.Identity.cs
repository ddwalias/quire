using System;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class DynamicMethod
{
    internal LocalVariableManager LocalVariableManager => _localVariableManager;

    public string Id => $"{_name}|{_idxId:###}";

    public override Delegate CreateDelegate(Type delegateType, object? target)
    {
        return _dynamicMethod.CreateDelegate(delegateType, target);
    }
}
