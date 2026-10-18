# Mpl.Run.Instruction

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunInstructionMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(OpCode)` | 0x06000352 | MemberGen: `m_op = op;` | Mechanical; Release IL equals Mathcad's | Verified |
| `EmitCode(ILGenerator)` | 0x06000353 | Calls `il.Emit(m_op)` | Directly compared with a recording generator; exact Release IL | Verified |
| `Op(OpCode)` | 0x06000354 | MemberGen: `new Instruction(op)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode, Type)` | 0x06000355 | MemberGen: `new InstructionArgType(op, ty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode, int)` | 0x06000356 | MemberGen: `new InstructionArgInt(op, i)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode, FieldInfo)` | 0x06000357 | MemberGen: `new InstructionArgField(op, f)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Op(OpCode, ConstructorInfo)` | 0x06000358 | MemberGen: `new InstructionArgCtor(op, c)` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgType: .ctor(OpCode, Type)` | 0x06000733 | MemberGen: `m_type = ty;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgType.EmitCode(ILGenerator)` | 0x06000734 | Calls `il.Emit(m_op, m_type)` | Directly compared with a recording generator; exact Release IL | Verified |
| `InstructionArgInt: .ctor(OpCode, int)` | 0x06000735 | MemberGen: `m_int = i;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgInt.EmitCode(ILGenerator)` | 0x06000736 | Calls `il.Emit(m_op, m_int)` | Directly compared with a recording generator; exact Release IL | Verified |
| `InstructionArgField: .ctor(OpCode, FieldInfo)` | 0x06000737 | MemberGen: `m_field = fi;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgField.EmitCode(ILGenerator)` | 0x06000738 | Calls `il.Emit(m_op, m_field)` | Directly compared with a recording generator; exact Release IL | Verified |
| `InstructionArgCtor: .ctor(OpCode, ConstructorInfo)` | 0x06000739 | MemberGen: `m_ctor = ci;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InstructionArgCtor.EmitCode(ILGenerator)` | 0x0600073a | Calls `il.Emit(m_op, m_ctor)` | Directly compared with a recording generator; exact Release IL | Verified |

## Test cases

**Observed** (`MplRunInstructionMechanicalDifferentialTests`): constructors and factories on generated inputs
(Mechanical: zero, null and distinct values per parameter and field), comparing results and target state.

**Observed** (`InstructionEmitCodeDifferentialTests`): directly invokes each `EmitCode` overload using a recording
subclass of each artifact's `ILGenerator`; records the opcode and exact type, integer, field, or constructor operand.
The sequence case emits every instruction form in order and compares recorder state with Mathcad's.
