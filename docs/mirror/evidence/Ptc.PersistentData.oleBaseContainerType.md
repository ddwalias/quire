# Ptc.PersistentData.oleBaseContainerType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060008a6 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OleViewportSize get` | 0x0600089b | MemberGen: `oleViewportSizeField; set => oleViewportSizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OleViewportSize set` | 0x0600089c | MemberGen: `oleViewportSizeField; set => oleViewportSizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StorageFileName get` | 0x0600089d | MemberGen: `public string StorageFileName { private get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StorageFileName set` | 0x0600089e | MemberGen: `public string StorageFileName { private get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `S11NProvider get` | 0x0600089f | MemberGen: `public IS11NProvider S11NProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `S11NProvider set` | 0x060008a0 | MemberGen: `public IS11NProvider S11NProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataoleBaseContainerTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
