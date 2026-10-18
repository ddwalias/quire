# Ptc.PersistentDataObjects.regionInWhiteboardBase

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsregionInWhiteboardBaseMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000a20 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `top get` | 0x06000a17 | MemberGen: `topField; set => topField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `top set` | 0x06000a18 | MemberGen: `topField; set => topField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `left get` | 0x06000a19 | MemberGen: `leftField; set => leftField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `left set` | 0x06000a1a | MemberGen: `leftField; set => leftField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC15TopOffset get` | 0x06000a1e | MemberGen: `public double MC15TopOffset { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC15TopOffset set` | 0x06000a1d | MemberGen: `public double MC15TopOffset { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsregionInWhiteboardBaseMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
