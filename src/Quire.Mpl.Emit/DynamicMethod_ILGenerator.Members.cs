using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class DynamicMethod_ILGenerator
{
    internal string Id => _dynamicMethodMaster.Id;

    public DynamicMethod_ILGenerator(System.Reflection.Emit.ILGenerator ilGenerator, DynamicMethod master)
    {
        _dynamicMethodMaster = master;
        _map = new System.Collections.Generic.Dictionary<EntityWrappers.LocalBuilderReferenceWrapper, System.Reflection.Emit.LocalBuilder>();
        _ilGenerator = ilGenerator ?? throw new ArgumentNullException("ilGenerator");
    }

    public override void Emit(OpCode opcode) => _ilGenerator.Emit(opcode);
    public override void Emit(OpCode opcode, byte arg) => _ilGenerator.Emit(opcode, arg);
    public override void Emit(OpCode opcode, double arg) => _ilGenerator.Emit(opcode, arg);
    public override void Emit(OpCode opcode, short arg) => _ilGenerator.Emit(opcode, arg);
    public override void Emit(OpCode opcode, int arg) => _ilGenerator.Emit(opcode, arg);
    public override void Emit(OpCode opcode, long arg) => _ilGenerator.Emit(opcode, arg);
    public override void Emit(OpCode opcode, float arg) => _ilGenerator.Emit(opcode, arg);
    public override void Emit(OpCode opcode, string str) => _ilGenerator.Emit(opcode, str);
    public override Label DefineLabel() => _ilGenerator.DefineLabel();
    public override void MarkLabel(Label lb) => _ilGenerator.MarkLabel(lb);
    public override void Emit(OpCode opcode, MethodInfo meth) => _ilGenerator.Emit(opcode, meth);
    public override void Emit(OpCode opcode, ConstructorInfo con) => _ilGenerator.Emit(opcode, con);
    public override void Emit(OpCode opcode, Type cls) => _ilGenerator.Emit(opcode, cls);
    public override void Emit(OpCode opcode, FieldInfo field) => _ilGenerator.Emit(opcode, field);
    public override void Emit(OpCode opcode, Label label) => _ilGenerator.Emit(opcode, label);
    public override void Emit(OpCode opcode, Label[] labels) => _ilGenerator.Emit(opcode, labels);
    public override void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[]? optionalParameterTypes) => _ilGenerator.EmitCall(opcode, methodInfo, optionalParameterTypes);
    public override void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes) => _ilGenerator.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
    public override void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes) => _ilGenerator.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes);

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
        EntityWrappers.LocalBuilderReferenceWrapper key = new EntityWrappers.LocalBuilderReferenceWrapper(local);
        if (!_map.ContainsKey(key))
        {
            throw new ArgumentException("LocalBuilder unknown");
        }
        _ilGenerator.Emit(opcode, _map[key]);
    }

    public override void BeginCatchBlock(Type? exceptionType) => _ilGenerator.BeginCatchBlock(exceptionType);
    public override void BeginExceptFilterBlock() => throw new NotImplementedException();
    public override Label BeginExceptionBlock() => _ilGenerator.BeginExceptionBlock();
    public override void EndExceptionBlock() => _ilGenerator.EndExceptionBlock();
}
