# Ptc.PersistentData.chartContainerType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDatachartContainerTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060008d9 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SizeInPixels get` | 0x060008d6 | MemberGen: `sizeInPixelsField; set => sizeInPixelsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SizeInPixels set` | 0x060008d7 | MemberGen: `sizeInPixelsField; set => sizeInPixelsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ExternalObjectType get` | 0x060008da | MemberGen: `ExternalObjectType.Chart` | Mechanical; Release IL equals Mathcad's | Verified |
| `AutoResize get` | 0x060008db | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasFakeOleViewportSize get` | 0x060008dd | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDatachartContainerTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
