using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class ILGenerator
{
    public virtual int ILOffset => throw new NotImplementedException();
    public virtual void BeginCatchBlock(Type? exceptionType) => throw new NotImplementedException();
    public virtual void BeginExceptFilterBlock() => throw new NotImplementedException();
    public virtual Label BeginExceptionBlock()
    {
        throw new NotImplementedException();
    }
    public virtual void BeginFaultBlock() => throw new NotImplementedException();
    public virtual void BeginFinallyBlock() => throw new NotImplementedException();
    public virtual void BeginScope() => throw new NotImplementedException();
    public virtual LocalBuilder DeclareLocal(Type localType, bool pinned = false) => throw new NotImplementedException();
    public virtual Label DefineLabel()
    {
        throw new NotImplementedException();
    }
    public virtual void Emit(OpCode opcode) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, byte arg) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, double arg) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, short arg) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, int arg) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, long arg) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, ConstructorInfo con) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, Label label) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, Label[] labels) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, LocalBuilder local) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, System.Reflection.Emit.SignatureHelper signature) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, FieldInfo field) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, MethodInfo meth) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, float arg) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, string str) => throw new NotImplementedException();
    public virtual void Emit(OpCode opcode, Type cls) => throw new NotImplementedException();
    public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[]? optionalParameterTypes) => throw new NotImplementedException();
    public virtual void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes) => throw new NotImplementedException();
    public virtual void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes) => throw new NotImplementedException();
    public virtual void EndExceptionBlock() => throw new NotImplementedException();
    public virtual void EndScope() => throw new NotImplementedException();
    public virtual void MarkLabel(Label loc) => throw new NotImplementedException();
    public virtual void UsingNamespace(string usingNamespace) => throw new NotImplementedException();
}
