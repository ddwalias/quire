# Ptc.PersistentDataObjects.plotEquation

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsplotEquationMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000cd5 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `math get` | 0x06000cd1 | MemberGen: `mathField; set => mathField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `math set` | 0x06000cd2 | MemberGen: `mathField; set => mathField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `formattingOverride get` | 0x06000cd3 | MemberGen: `formattingOverrideField; set => formattingOverrideField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `formattingOverride set` | 0x06000cd4 | MemberGen: `formattingOverrideField; set => formattingOverrideField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsplotEquationMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
