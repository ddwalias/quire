# Ptc.Controls.RegularSaveOptions

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsRegularSaveOptionsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000164 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TryGeneratingInputOutputStates get` | 0x06000165 | MemberGen: `public bool TryGeneratingInputOutputStates { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TryGeneratingInputOutputStates set` | 0x06000166 | MemberGen: `public bool TryGeneratingInputOutputStates { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsDetachedCopy get` | 0x06000167 | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsHandlingException get` | 0x06000168 | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `WaitForCalculationForSaveAsApi get` | 0x06000169 | MemberGen: `public bool WaitForCalculationForSaveAsApi { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `WaitForCalculationForSaveAsApi set` | 0x0600016a | MemberGen: `public bool WaitForCalculationForSaveAsApi { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsRegularSaveOptionsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
