# Ptc.Calculation.Core.EngineRedefInfo

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreEngineRedefInfoMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `OldLocation get` | 0x06000049 | MemberGen: `public string OldLocation { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OldLocation set` | 0x0600004a | MemberGen: `public string OldLocation { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OldType get` | 0x0600004b | MemberGen: `public EngineDefType OldType { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OldType set` | 0x0600004c | MemberGen: `public EngineDefType OldType { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x0600004d | MemberGen: `public string Name { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name set` | 0x0600004e | MemberGen: `public string Name { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreEngineRedefInfoMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
