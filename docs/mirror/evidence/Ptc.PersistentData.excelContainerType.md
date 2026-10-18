# Ptc.PersistentData.excelContainerType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataexcelContainerTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060008a7 | MemberGen: `isIconField = false;` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsIcon get` | 0x060008a8 | MemberGen: `isIconField; set => isIconField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsIcon set` | 0x060008a9 | MemberGen: `isIconField; set => isIconField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ExternalObjectType get` | 0x060008ab | MemberGen: `ExternalObjectType.Excel` | Mechanical; Release IL equals Mathcad's | Verified |
| `AutoResize get` | 0x060008ac | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasFakeOleViewportSize get` | 0x060008ae | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataexcelContainerTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
