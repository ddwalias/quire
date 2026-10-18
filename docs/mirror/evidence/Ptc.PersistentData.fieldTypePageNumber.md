# Ptc.PersistentData.fieldTypePageNumber

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDatafieldTypePageNumberMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000931 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `template get` | 0x0600092b | MemberGen: `templateField; set => templateField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `template set` | 0x0600092c | MemberGen: `templateField; set => templateField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `templateSpecified get` | 0x0600092d | MemberGen: `templateFieldSpecified; set => templateFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `templateSpecified set` | 0x0600092e | MemberGen: `templateFieldSpecified; set => templateFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDatafieldTypePageNumberMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
