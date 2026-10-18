# Ptc.PersistentData.worksheet

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataworksheetMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600093e | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions get` | 0x06000932 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions set` | 0x06000933 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convertedfrom get` | 0x06000934 | MemberGen: `convertedfromField; set => convertedfromField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convertedfrom set` | 0x06000935 | MemberGen: `convertedfromField; set => convertedfromField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `msgId get` | 0x06000936 | MemberGen: `msgIdField; set => msgIdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `msgId set` | 0x06000937 | MemberGen: `msgIdField; set => msgIdField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convertedfromSpecified get` | 0x06000938 | MemberGen: `convertedfromFieldSpecified; set => convertedfromFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convertedfromSpecified set` | 0x06000939 | MemberGen: `convertedfromFieldSpecified; set => convertedfromFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MsgId get` | 0x0600093d | MemberGen: `msgId` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataworksheetMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
