using System;
using System.Globalization;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class DynamicMethod
{
    internal LocalVariableManager LocalVariableManager => _localVariableManager;

    public string Id => $"{_name}|{_idxId:###}";

    public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();

    public override MethodAttributes Attributes => throw new NotImplementedException();

    public DynamicMethod(string name, Type? returnType, Type[]? parameters, Type declaringType)
    {
        _dynamicMethod = new System.Reflection.Emit.DynamicMethod(name, returnType, parameters, declaringType);
        _name = name;
        _idxId = _s_counter++;
        _ilGenerator = null;
        _localVariableManager = new LocalVariableManager(this);
    }

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

    private readonly System.Collections.Generic.Dictionary<Func<DynamicMethod, bool>, Func<DynamicMethod, DynamicMethod_ILGenerator>> extra_creators = new System.Collections.Generic.Dictionary<Func<DynamicMethod, bool>, Func<DynamicMethod, DynamicMethod_ILGenerator>>
    {
        {
            (DynamicMethod t) => t._name.Contains("$$157_") && false,
            (DynamicMethod t) => new DynamicMethod_ILGenerator_Special(t._dynamicMethod.GetILGenerator(), t)
        },
        {
            (DynamicMethod t) => t._name.Contains("$k0_") && false,
            (DynamicMethod t) => new DynamicMethod_ILGenerator_Special(t._dynamicMethod.GetILGenerator(), t)
        }
    };
}
