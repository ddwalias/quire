#nullable enable

namespace System.Reflection.Emit.Quire.Mpl;

public partial record ILInstruction(
    ILCallsEncoding CallCommand,
    OpCode OpCode,
    object? Operand = null,
    Type? decalaredType = null,
    Type? returnType = null,
    Type[]? parameterTypes = null,
    Type[]? optionalParameterTypes = null);
