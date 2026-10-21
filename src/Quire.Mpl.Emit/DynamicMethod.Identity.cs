using System;
using System.Globalization;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class DynamicMethod
{
    public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();

    public override MethodAttributes Attributes => throw new NotImplementedException();

    public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();

    public override Type? DeclaringType => throw new NotImplementedException();

    public override string Name => throw new NotImplementedException();

    public override Type? ReflectedType => throw new NotImplementedException();

    public override MethodInfo GetBaseDefinition() => throw new NotImplementedException();

    public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();

    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotImplementedException();

    public override MethodImplAttributes GetMethodImplementationFlags() => throw new NotImplementedException();

    public override ParameterInfo[] GetParameters() => throw new NotImplementedException();

    public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException();

    internal LocalVariableManager LocalVariableManager => _localVariableManager;

    public string Id => $"{_name}|{_idxId:###}";

    public override Delegate CreateDelegate(Type delegateType, object? target)
    {
        return _dynamicMethod.CreateDelegate(delegateType, target);
    }

    public ILGenerator GetILGenerator()
    {
        if (_ilGenerator == null)
        {
            _ilGenerator = new DynamicMethod_ILGenerator(_dynamicMethod.GetILGenerator(), this);
        }
        return _ilGenerator;
    }
}
