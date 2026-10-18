# Ptc.Calculation.Core.EngineError

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreEngineErrorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600003f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorCode get` | 0x06000028 | MemberGen: `_code` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorMessageArguments get` | 0x06000029 | MemberGen: `_arguments` | Mechanical; Release IL equals Mathcad's | Verified |
| `NodeHandle get` | 0x0600002a | MemberGen: `public uint NodeHandle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NodeHandle set` | 0x0600002b | MemberGen: `public uint NodeHandle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument get` | 0x0600002c | MemberGen: `public uint Argument { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument set` | 0x0600002d | MemberGen: `public uint Argument { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResourceString get` | 0x0600002e | MemberGen: `public string ResourceString { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResourceString set` | 0x0600002f | MemberGen: `public string ResourceString { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OriginalError get` | 0x06000030 | MemberGen: `public EngineError OriginalError { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OriginalError set` | 0x06000031 | MemberGen: `public EngineError OriginalError { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsRuntime get` | 0x06000036 | MemberGen: `public bool IsRuntime { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsRuntime set` | 0x06000037 | MemberGen: `public bool IsRuntime { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorSource get` | 0x06000038 | MemberGen: `public ICalculatable ErrorSource { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorSource set` | 0x06000039 | MemberGen: `public ICalculatable ErrorSource { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreEngineErrorMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
