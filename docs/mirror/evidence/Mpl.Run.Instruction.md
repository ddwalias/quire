# Mpl.Run.Instruction

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's. `EmitCode` bodies were reconstructed from `Mpl.Run.Instruction` IL.
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunInstructionMechanicalDifferentialTests`, `Quire.Differential.Tests.MplRunInstructionEmitDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(OpCode)` | 0x06000352 | MemberGen: `m_op = op;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode)` | 0x06000354 | MemberGen: `new Instruction(op)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode, Type)` | 0x06000355 | MemberGen: `new InstructionArgType(op, ty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode, int)` | 0x06000356 | MemberGen: `new InstructionArgInt(op, i)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode, FieldInfo)` | 0x06000357 | MemberGen: `new InstructionArgField(op, f)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode, ConstructorInfo)` | 0x06000358 | MemberGen: `new InstructionArgCtor(op, c)` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgType: .ctor(OpCode, Type)` | 0x06000733 | MemberGen: `m_type = ty;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgInt: .ctor(OpCode, int)` | 0x06000735 | MemberGen: `m_int = i;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgField: .ctor(OpCode, FieldInfo)` | 0x06000737 | MemberGen: `m_field = fi;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgCtor: .ctor(OpCode, ConstructorInfo)` | 0x06000739 | MemberGen: `m_ctor = ci;` | Mechanical; Release IL equals Mathcad's | Verified |
| `EmitCode(ILGenerator)` | 0x06000353 | `il.Emit(m_op);` | High; directly compared for `nop`. Other opcodes are not established. | Verified |
| `InstructionArgType: EmitCode(ILGenerator)` | 0x06000734 | `il.Emit(m_op, m_type);` | High; directly compared for `box System.Int32`. Other type opcodes/types are not established. | Verified |
| `InstructionArgInt: EmitCode(ILGenerator)` | 0x06000736 | `il.Emit(m_op, m_int);` | High; directly compared for `ldc.i4 19`. Other integer operands/opcodes are not established. | Verified |
| `InstructionArgField: EmitCode(ILGenerator)` | 0x06000738 | `il.Emit(m_op, m_field);` | High; directly compared for `ldfld EmitterSample.Value`. Other fields/opcodes are not established. | Verified |
| `InstructionArgCtor: EmitCode(ILGenerator)` | 0x0600073a | `il.Emit(m_op, m_ctor);` | High; directly compared for `newobj EmitterSample()`. Other constructors/opcodes are not established. | Verified |

## Test cases

**Observed** (`MplRunInstructionMechanicalDifferentialTests`): each constructor and `Op` overload on generated inputs (Mechanical: zero,
null and distinct values per parameter and field), comparing the result, the arguments and target fields with Mathcad's.

**Observed** (`MplRunInstructionEmitDifferentialTests`): each `EmitCode` method is invoked directly on both artifacts. The Quire side
uses a recording `ILGenerator`; the Mathcad side uses `mplasb`'s `ILGeneratorStore`. Captured opcodes and operands are compared for
`nop`, `box System.Int32`, `ldc.i4 19`, `ldfld EmitterSample.Value`, and `newobj EmitterSample()`. The sequence case compares those
five operations in that order. These cases do not establish other operands or generator implementations.
