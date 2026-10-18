# Ptc.PersistentData.headerFooterRegions

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000945 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions get` | 0x0600093f | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions set` | 0x06000940 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convertedfrom get` | 0x06000941 | MemberGen: `convertedfromField; set => convertedfromField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convertedfrom set` | 0x06000942 | MemberGen: `convertedfromField; set => convertedfromField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convertedfromSpecified get` | 0x06000943 | MemberGen: `convertedfromFieldSpecified; set => convertedfromFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convertedfromSpecified set` | 0x06000944 | MemberGen: `convertedfromFieldSpecified; set => convertedfromFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataheaderFooterRegionsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
