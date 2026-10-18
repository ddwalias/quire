# Ptc.Controls.IO.MatrixDefinition

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsIOMatrixDefinitionMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Rl get` | 0x060001e7 | MemberGen: `_rl; set => _rl = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rl set` | 0x060001e8 | MemberGen: `_rl; set => _rl = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IdSExpression get` | 0x060001e9 | MemberGen: `public string IdSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IdSExpression set` | 0x060001ea | MemberGen: `public string IdSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x060001eb | MemberGen: `public double[,] Value { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x060001ec | MemberGen: `public double[,] Value { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalingSExpression get` | 0x060001ed | MemberGen: `public string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalingSExpression set` | 0x060001ee | MemberGen: `public string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValueSExpression get` | 0x060001f6 | MemberGen: `public string ValueSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValueSExpression set` | 0x060001f7 | MemberGen: `public string ValueSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsIOMatrixDefinitionMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
