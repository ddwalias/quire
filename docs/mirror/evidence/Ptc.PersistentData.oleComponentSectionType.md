# Ptc.PersistentData.oleComponentSectionType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060008c6 | MemberGen: `visibilityField = OleComponentSectionVisibility.Visible;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visibility get` | 0x060008c7 | MemberGen: `visibilityField; set => visibilityField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visibility set` | 0x060008c8 | MemberGen: `visibilityField; set => visibilityField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Height get` | 0x060008c9 | MemberGen: `heightField; set => heightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Height set` | 0x060008ca | MemberGen: `heightField; set => heightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HeightSpecified get` | 0x060008cb | MemberGen: `heightFieldSpecified; set => heightFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HeightSpecified set` | 0x060008cc | MemberGen: `heightFieldSpecified; set => heightFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataoleComponentSectionTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
