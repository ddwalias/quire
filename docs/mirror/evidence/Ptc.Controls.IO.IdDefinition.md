# Ptc.Controls.IO.IdDefinition

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsIOIdDefinitionMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Rl get` | 0x060001d6 | MemberGen: `_rl; set => _rl = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rl set` | 0x060001d7 | MemberGen: `_rl; set => _rl = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IdSExpression get` | 0x060001d8 | MemberGen: `public string IdSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IdSExpression set` | 0x060001d9 | MemberGen: `public string IdSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x060001da | MemberGen: `public double Value { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x060001db | MemberGen: `public double Value { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalingSExpression get` | 0x060001dc | MemberGen: `public string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalingSExpression set` | 0x060001dd | MemberGen: `public string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValueSExpression get` | 0x060001de | MemberGen: `public string ValueSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValueSExpression set` | 0x060001df | MemberGen: `public string ValueSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsIOIdDefinitionMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
