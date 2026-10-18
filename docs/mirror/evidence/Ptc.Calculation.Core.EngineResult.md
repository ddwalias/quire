# Ptc.Calculation.Core.EngineResult

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreEngineResultMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ResultNode get` | 0x06000058 | MemberGen: `public IResultNode ResultNode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResultNode set` | 0x06000059 | MemberGen: `public IResultNode ResultNode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Errors get` | 0x0600005a | MemberGen: `public EngineErrorCollection Errors { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Errors set` | 0x0600005b | MemberGen: `public EngineErrorCollection Errors { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Warnings get` | 0x0600005c | MemberGen: `public EngineRedefInfo[] Warnings { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Warnings set` | 0x0600005d | MemberGen: `public EngineRedefInfo[] Warnings { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SynchronizationStatus get` | 0x0600005e | MemberGen: `public SynchronizationStatus SynchronizationStatus { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SynchronizationStatus set` | 0x0600005f | MemberGen: `public SynchronizationStatus SynchronizationStatus { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SerializationTag get` | 0x06000062 | MemberGen: `public string SerializationTag { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SerializationTag set` | 0x06000063 | MemberGen: `public string SerializationTag { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreEngineResultMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
