using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class DynamicMethod_ILGenerator_Special
{
    public DynamicMethod_ILGenerator_Special(System.Reflection.Emit.ILGenerator ilGenerator, DynamicMethod master)
        : base(ilGenerator, master)
    {
    }

    public override void Emit(OpCode opcode)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode);
        }
    }

    public override void Emit(OpCode opcode, byte arg)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, arg);
        }
    }

    public override void Emit(OpCode opcode, double arg)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, arg);
        }
    }

    public override void Emit(OpCode opcode, short arg)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, arg);
        }
    }

    public override void Emit(OpCode opcode, int arg)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, arg);
        }
    }

    public override void Emit(OpCode opcode, long arg)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, arg);
        }
    }

    public override void Emit(OpCode opcode, float arg)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, arg);
        }
    }

    public override void Emit(OpCode opcode, string str)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, str);
        }
    }

    public override Label DefineLabel() => _ilGenerator.DefineLabel();

    public override void MarkLabel(Label lb) => _ilGenerator.MarkLabel(lb);

    public static object Probe0(object arg1)
    {
        string text = (string)arg1;
        Logger.Log(">>> Probe0 " + text, "D:\\JAgent\\workspace\\Prime_12.0_Mathcad_Engine_Build_Release_And_Test\\Mathcad_Engine\\mpl\\mplasb\\DynamicMethod.cs", "Probe0", 493);
        return new object();
    }

    public static object Probe1(object arg1)
    {
        string text = (string)arg1;
        Logger.Log(">>> Probe1 " + text, "D:\\JAgent\\workspace\\Prime_12.0_Mathcad_Engine_Build_Release_And_Test\\Mathcad_Engine\\mpl\\mplasb\\DynamicMethod.cs", "Probe1", 499);
        return new object();
    }

    public static object Probe_Int32_Object(int arg1, object arg2) => arg2;

    public override void Emit(OpCode opcode, MethodInfo meth)
    {
        if (!_ignore_call)
        {
            if (meth.Name != null)
            {
                MethodInfo method = typeof(DynamicMethod_ILGenerator_Special).GetMethod("Probe0", BindingFlags.Static | BindingFlags.Public, null, new Type[1] { typeof(object) }, null);
                _ilGenerator.Emit(OpCodes.Ldstr, meth.DeclaringType.FullName + "::" + meth.Name);
                Logger.Log($"IlGenerator {Id} opcode = {OpCodes.Ldstr}, str = {meth.Name}", "D:\\JAgent\\workspace\\Prime_12.0_Mathcad_Engine_Build_Release_And_Test\\Mathcad_Engine\\mpl\\mplasb\\DynamicMethod.cs", "Emit", 528);
                _ilGenerator.Emit(OpCodes.Call, method);
                Logger.Log($"IlGenerator {Id} opcode = {OpCodes.Call} , MethodInfo = {LoggerAssistant.GetFullNameWithAssembly(method)}", "D:\\JAgent\\workspace\\Prime_12.0_Mathcad_Engine_Build_Release_And_Test\\Mathcad_Engine\\mpl\\mplasb\\DynamicMethod.cs", "Emit", 529);
                _ilGenerator.Emit(OpCodes.Pop);
                Logger.Log($"IlGenerator {Id} opcode = {OpCodes.Pop}", "D:\\JAgent\\workspace\\Prime_12.0_Mathcad_Engine_Build_Release_And_Test\\Mathcad_Engine\\mpl\\mplasb\\DynamicMethod.cs", "Emit", 531);
            }
            _ilGenerator.Emit(opcode, meth);
            if (meth.Name != null)
            {
                MethodInfo method2 = typeof(DynamicMethod_ILGenerator_Special).GetMethod("Probe1", BindingFlags.Static | BindingFlags.Public, null, new Type[1] { typeof(object) }, null);
                _ilGenerator.Emit(OpCodes.Ldstr, meth.DeclaringType.FullName + "::" + meth.Name);
                Logger.Log($"IlGenerator {Id} opcode = {OpCodes.Ldstr}, str = {meth.Name}", "D:\\JAgent\\workspace\\Prime_12.0_Mathcad_Engine_Build_Release_And_Test\\Mathcad_Engine\\mpl\\mplasb\\DynamicMethod.cs", "Emit", 567);
                _ilGenerator.Emit(OpCodes.Call, method2);
                Logger.Log($"IlGenerator {Id} opcode = {OpCodes.Call} , MethodInfo = {LoggerAssistant.GetFullNameWithAssembly(method2)}", "D:\\JAgent\\workspace\\Prime_12.0_Mathcad_Engine_Build_Release_And_Test\\Mathcad_Engine\\mpl\\mplasb\\DynamicMethod.cs", "Emit", 568);
                _ilGenerator.Emit(OpCodes.Pop);
                Logger.Log($"IlGenerator {Id} opcode = {OpCodes.Pop}", "D:\\JAgent\\workspace\\Prime_12.0_Mathcad_Engine_Build_Release_And_Test\\Mathcad_Engine\\mpl\\mplasb\\DynamicMethod.cs", "Emit", 570);
            }
        }
    }

    public override void Emit(OpCode opcode, ConstructorInfo con)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, con);
        }
    }

    public override void Emit(OpCode opcode, Type cls)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, cls);
        }
    }

    public override void Emit(OpCode opcode, FieldInfo field)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, field);
        }
    }

    public override void Emit(OpCode opcode, Label label)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, label);
        }
    }

    public override void Emit(OpCode opcode, Label[] labels)
    {
        if (!_ignore_call)
        {
            _ilGenerator.Emit(opcode, labels);
        }
    }

    public override void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[]? optionalParameterTypes)
    {
        if (!_ignore_call)
        {
            _ilGenerator.EmitCall(opcode, methodInfo, optionalParameterTypes);
        }
    }

    public override void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes)
        => _ilGenerator.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);

    public override void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
    {
        if (!_ignore_call)
        {
            _ilGenerator.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes);
        }
    }

    public override LocalBuilder DeclareLocal(Type localType, bool pinned = false)
    {
        LocalBuilder localBuilder = _dynamicMethodMaster.LocalVariableManager.DeclareLocal(localType, pinned);
        System.Reflection.Emit.LocalBuilder value = _ilGenerator.DeclareLocal(localType, pinned);
        EntityWrappers.LocalBuilderReferenceWrapper key = new EntityWrappers.LocalBuilderReferenceWrapper(localBuilder);
        if (_map.ContainsKey(key))
        {
            throw new ArgumentException("LocalBuilder already exists");
        }
        _map[key] = value;
        return localBuilder;
    }

    public override void Emit(OpCode opcode, LocalBuilder local)
    {
        if (!_ignore_call)
        {
            EntityWrappers.LocalBuilderReferenceWrapper key = new EntityWrappers.LocalBuilderReferenceWrapper(local);
            if (!_map.ContainsKey(key))
            {
                throw new ArgumentException("LocalBuilder unknown");
            }
            _ilGenerator.Emit(opcode, _map[key]);
        }
    }

    public override void BeginCatchBlock(Type? exceptionType)
    {
        _ignore_call = false;
        _ilGenerator.BeginCatchBlock(exceptionType);
    }

    public override void BeginExceptFilterBlock() => throw new NotImplementedException();
    public override Label BeginExceptionBlock() => _ilGenerator.BeginExceptionBlock();
    public override void EndExceptionBlock() => _ilGenerator.EndExceptionBlock();
}
