# Ptc.PersistentData.oleObjectRegionType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060008d0 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AutoResize get` | 0x060008cd | MemberGen: `autoResizeField; set => autoResizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AutoResize set` | 0x060008ce | MemberGen: `autoResizeField; set => autoResizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ExternalObjectType get` | 0x060008d1 | MemberGen: `ExternalObjectType.Ole` | Mechanical; Release IL equals Mathcad's | Verified |
| `FakeOleViewportSize get` | 0x060008d3 | MemberGen: `public Nullable<Size> FakeOleViewportSize { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FakeOleViewportSize set` | 0x060008d4 | MemberGen: `public Nullable<Size> FakeOleViewportSize { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataoleObjectRegionTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
