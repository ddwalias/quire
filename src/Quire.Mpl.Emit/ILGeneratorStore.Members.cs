using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class ILGeneratorStore
{
    private List<ILInstruction> instructions;
    public List<ILInstruction> Instructions => instructions;

    public override int ILOffset => throw new NotImplementedException();

    internal ILGeneratorStore(MethodBase method, CommonMemberData commonMemberData)
    {
        instructions = new List<ILInstruction>();
        _method = method;
        _memberData = commonMemberData;
    }

    public override void BeginCatchBlock(Type? exceptionType)
    {
        Instructions.Add(new ILInstruction(ILCallsEncoding.eBeginCatchBlock, OpCodes.Nop, exceptionType, typeof(Type)));
    }

    public override void BeginExceptFilterBlock() => throw new NotImplementedException();

    public override Label BeginExceptionBlock()
    {
        Label label = _memberData.labelManager.DefineLabel();
        Instructions.Add(new ILInstruction(ILCallsEncoding.eBeginExceptionBlock, OpCodes.Nop, label, typeof(Label)));
        return label;
    }

    public override void BeginFaultBlock() => throw new NotImplementedException();
    public override void BeginFinallyBlock() => throw new NotImplementedException();
    public override void BeginScope() => throw new NotImplementedException();
    public override LocalBuilder DeclareLocal(Type localType, bool pinned = false) => _memberData.localVars.DeclareLocal(localType, pinned);

    public override Label DefineLabel()
    {
        Label label = _memberData.labelManager.DefineLabel();
        Instructions.Add(new ILInstruction(ILCallsEncoding.eDeclareLabel, OpCodes.Nop, label, typeof(Label)));
        return label;
    }

    public override void Emit(OpCode opcode) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode));
    public override void Emit(OpCode opcode, byte arg) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, arg, typeof(byte)));
    public override void Emit(OpCode opcode, double arg) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, arg));
    public override void Emit(OpCode opcode, short arg) => throw new NotImplementedException();
    public override void Emit(OpCode opcode, int arg) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, arg));
    public override void Emit(OpCode opcode, long arg) => throw new NotImplementedException();
    public override void Emit(OpCode opcode, ConstructorInfo ctor) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, ctor, typeof(ConstructorInfo)));
    public override void Emit(OpCode opcode, Label label) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, label, typeof(Label)));
    public override void Emit(OpCode opcode, Label[] labels) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, labels, typeof(Label[])));
    public override void Emit(OpCode opcode, LocalBuilder local) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, local, typeof(LocalBuilder)));
    public override void Emit(OpCode opcode, System.Reflection.Emit.SignatureHelper signature) => throw new NotImplementedException();
    public override void Emit(OpCode opcode, FieldInfo field) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, field, typeof(FieldInfo)));
    public override void Emit(OpCode opcode, MethodInfo methodInfo) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, methodInfo, typeof(MethodInfo)));
    public override void Emit(OpCode opcode, float arg) => throw new NotImplementedException();
    public override void Emit(OpCode opcode, string str) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, str, typeof(string)));
    public override void Emit(OpCode opcode, Type cls) => instructions.Add(new ILInstruction(ILCallsEncoding.eEmit, opcode, cls, typeof(Type)));

    public override void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[]? optionalParameterTypes)
    {
        instructions.Add(new ILInstruction(ILCallsEncoding.eEmitCall, opcode, methodInfo, null, null, null, optionalParameterTypes));
    }

    public override void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes)
        => throw new NotImplementedException();

    public override void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
        => throw new NotImplementedException();

    public override void EndExceptionBlock() => Instructions.Add(new ILInstruction(ILCallsEncoding.eEndExceptionBlock, OpCodes.Nop));
    public override void EndScope() => throw new NotImplementedException();
    public override void MarkLabel(Label label) => Instructions.Add(new ILInstruction(ILCallsEncoding.eMarkLabel, OpCodes.Nop, label, typeof(Label)));
    public override void UsingNamespace(string usingNamespace) => throw new NotImplementedException();
}
