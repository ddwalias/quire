# Ptc.Controls.Core.FileLoadResult

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreFileLoadResultMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060002bd | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `LoadException get` | 0x060002b8 | MemberGen: `public Exception LoadException { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `LoadException set` | 0x060002b7 | MemberGen: `public Exception LoadException { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorsOccurredWhenLoadingRegions get` | 0x060002b9 | MemberGen: `public bool ErrorsOccurredWhenLoadingRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorsOccurredWhenLoadingRegions set` | 0x060002ba | MemberGen: `public bool ErrorsOccurredWhenLoadingRegions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializationExceptions get` | 0x060002bb | MemberGen: `public IEnumerable<Exception> DeserializationExceptions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializationExceptions set` | 0x060002bc | MemberGen: `public IEnumerable<Exception> DeserializationExceptions { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreFileLoadResultMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
