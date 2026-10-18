# Ptc.Controls.IO.ScaledEvaluation

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsIOScaledEvaluationMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `IsErrorOnScalingUnits()` | 0x0600020b | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalingSExpression get` | 0x06000200 | MemberGen: `public string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalingSExpression set` | 0x06000201 | MemberGen: `public string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaseSExpression get` | 0x06000202 | MemberGen: `public string BaseSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaseSExpression set` | 0x06000203 | MemberGen: `public string BaseSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rl get` | 0x06000204 | MemberGen: `_rl; set => _rl = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rl set` | 0x06000205 | MemberGen: `_rl; set => _rl = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsIOScaledEvaluationMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
